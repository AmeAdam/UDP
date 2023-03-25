// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Security.Cryptography.X509Certificates;
using Grpc.Net.Client;
using GrpcService;


Console.WriteLine("Connecting...");

var httpClientHandler = new HttpClientHandler();
//httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
var httpClient = new HttpClient(httpClientHandler);
httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
httpClient.DefaultRequestVersion = HttpVersion.Version30;

var str = await httpClient.GetStringAsync("https://ame:18080");
Console.WriteLine(str);

var channel = GrpcChannel.ForAddress("https://ame:18080",
    new GrpcChannelOptions { HttpClient = httpClient });

//var channel = GrpcChannel.ForAddress("https://service1:18080");
var client = new Greeter.GreeterClient(channel);
var response = client.SayHello(new HelloRequest{Name = "Abc"});
Console.WriteLine(response.Message);
channel.Dispose();
