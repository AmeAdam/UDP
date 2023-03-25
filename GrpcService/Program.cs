using GrpcService.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "AME Witamy!");

app.Run();