using Lamp.Services;
using Lamp.Ports;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:" + (Environment.GetEnvironmentVariable("DEVICE_PORT") ?? "8080"));

builder.Services.AddControllers();
builder.Services.AddSingleton<ServerCommunicationProtocolHttpAdapter>();
builder.Services.AddSingleton<ILampService, LampService>();

builder.Services.AddHostedService(provider => 
    (LampService)provider.GetRequiredService<ILampService>());

var app = builder.Build();

// Map controller routes
app.MapControllers();

app.Run();
