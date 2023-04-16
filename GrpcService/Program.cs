using GrpcService;
using GrpcService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

var app = builder.Build();

//Klasyczne HTTP
app.MapGet("/", () => "Witamy HTTP GET!");

//GRPC
app.MapGrpcService<GreeterService>();

//raw QUIC 
builder.Services.AddHostedService<QuicServer>();

app.Run();