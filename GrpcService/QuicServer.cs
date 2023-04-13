using System.Net;
using System.Net.Quic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace GrpcService;

public class QuicServer : BackgroundService
{
    private ILogger<QuicServer> loger;

    public QuicServer(ILogger<QuicServer> loger)
    {
        this.loger = loger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var cert = new X509Certificate2("/certs/ame.pfx", "");
        
        loger.LogInformation($"Strating QUIC listener with Certificate: {cert.FriendlyName} {cert.Thumbprint} {cert.SubjectName.Name} {cert}");
        
        var serverConnectionOptions = new QuicServerConnectionOptions()
        {
            DefaultStreamErrorCode = 0x0A,
            DefaultCloseErrorCode = 0x0B,
            ServerAuthenticationOptions = new SslServerAuthenticationOptions
            {
                ApplicationProtocols = new List<SslApplicationProtocol> { SslApplicationProtocol.Http3 },
                ServerCertificate = cert
            }
        };
        
        var listener = await QuicListener.ListenAsync(new QuicListenerOptions
        {
            ListenEndPoint = new IPEndPoint(IPAddress.Any, 18082),
            ApplicationProtocols = new List<SslApplicationProtocol> { SslApplicationProtocol.Http3 },
            ConnectionOptionsCallback = (_, _, _) => ValueTask.FromResult(serverConnectionOptions)
        });
        loger.LogInformation("Waiting for QUIC connection");
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var connection = await listener.AcceptConnectionAsync(cancellationToken);
                var stream = await connection.AcceptInboundStreamAsync(cancellationToken);
                loger.LogInformation("QUIC connected");

                var buf = new byte[128];
                var len = await stream.ReadAsync(buf, 0, buf.Length);
                Console.WriteLine(Encoding.UTF8.GetString(buf, 0, len));
                
                buf = "Witamy QUIC"u8.ToArray();
                await stream.WriteAsync(buf, 0, buf.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        loger.LogInformation("QUIC finished");
        await listener.DisposeAsync();

    }

}