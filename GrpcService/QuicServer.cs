using System.Net;
using System.Net.Quic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace GrpcService;

#pragma warning disable CA1416
public class QuicServer : BackgroundService
{
    private readonly ILogger<QuicServer> logger;
    private readonly QuicServerConnectionOptions serverConnectionOptions;

    public QuicServer(ILogger<QuicServer> logger)
    {
        this.logger = logger;
        var cert = new X509Certificate2("/certs/ame.pfx", "");
        
        serverConnectionOptions = new QuicServerConnectionOptions
        {
            DefaultStreamErrorCode = 0x0A,
            DefaultCloseErrorCode = 0x0B,
            ServerAuthenticationOptions = new SslServerAuthenticationOptions
            {
                ApplicationProtocols = new List<SslApplicationProtocol> { SslApplicationProtocol.Http3 },
                ServerCertificate = cert
            }
        };
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var listener = await QuicListener.ListenAsync(new QuicListenerOptions
        {
            ListenEndPoint = new IPEndPoint(IPAddress.Any, 18082),
            ApplicationProtocols = new List<SslApplicationProtocol> { SslApplicationProtocol.Http3 },
            ConnectionOptionsCallback = (connection,clientHello, cancellation) =>
            {
                logger.LogInformation("Received Hello Packet from {ClientIP}", connection.RemoteEndPoint);
                if (clientHello.ServerName == "ame")
                    return ValueTask.FromResult(serverConnectionOptions);
                connection.DisposeAsync();
                throw new Exception();
            }
        }, cancellationToken);
        
        logger.LogInformation("Waiting for QUIC connection");
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var connection = await listener.AcceptConnectionAsync(cancellationToken);
                var stream = await connection.AcceptInboundStreamAsync(cancellationToken);
                logger.LogInformation("QUIC connected, {ClientAddress}", connection.RemoteEndPoint);

                var buf = new byte[128];
                var len = await stream.ReadAsync(buf, 0, buf.Length, cancellationToken);
                Console.WriteLine(Encoding.UTF8.GetString(buf, 0, len));
                
                buf = "Witamy QUIC"u8.ToArray();
                await stream.WriteAsync(buf, 0, buf.Length, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        logger.LogInformation("QUIC finished");
        await listener.DisposeAsync();

    }
}
#pragma warning restore CA1416