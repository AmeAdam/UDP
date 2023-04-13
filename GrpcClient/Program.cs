// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Quic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Grpc.Net.Client;
using GrpcService;

var remoteHost = "ame";

Console.WriteLine("HttpClient...");

var httpClientHandler = new HttpClientHandler();
//httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
var httpClient = new HttpClient(httpClientHandler);
httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
httpClient.DefaultRequestVersion = HttpVersion.Version30;
var httpResponseText  = await httpClient.GetStringAsync($"https://{remoteHost}:18081");
Console.WriteLine(httpResponseText);

Console.WriteLine("GRPC...");
var channel = GrpcChannel.ForAddress($"https://{remoteHost}:18081", new GrpcChannelOptions { HttpClient = httpClient });
var client = new Greeter.GreeterClient(channel);
var response = client.SayHello(new HelloRequest{Name = "Abc"});
Console.WriteLine(response.Message);
channel.Dispose();


Console.WriteLine("QUIC...");

var port = 18082;
var clientConnectionOptions = new QuicClientConnectionOptions
{
    RemoteEndPoint = new DnsEndPoint(remoteHost, port),
    DefaultStreamErrorCode = 0x0A,
    DefaultCloseErrorCode = 0x0B,
    MaxInboundUnidirectionalStreams = 10,
    MaxInboundBidirectionalStreams = 100,
    ClientAuthenticationOptions = new SslClientAuthenticationOptions
    {
        ApplicationProtocols = new List<SslApplicationProtocol> { SslApplicationProtocol.Http3 },
        TargetHost = remoteHost
    }
};
var connection = await QuicConnection.ConnectAsync(clientConnectionOptions);

var outgoingStream = await connection.OpenOutboundStreamAsync(QuicStreamType.Bidirectional);
var buf = "Hello QUIC"u8.ToArray();
await outgoingStream.WriteAsync(buf, 0, buf.Length);

buf = new byte[128];
var len = await outgoingStream.ReadAsync(buf, 0, buf.Length);
Console.WriteLine(Encoding.UTF8.GetString(buf, 0, len));

await connection.CloseAsync(0x0C);
await connection.DisposeAsync();