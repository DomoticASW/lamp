using Lamp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:8080");

// Register controllers
builder.Services.AddControllers();
builder.Services.AddSingleton<ILampService, LampService>();

var app = builder.Build();

// Map controller routes
app.MapControllers();

app.Run();