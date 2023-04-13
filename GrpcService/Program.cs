using GrpcService;
using GrpcService.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddGrpc();
builder.Services.AddHostedService<QuicServer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "Witamy HTTP GET!");

app.Run();