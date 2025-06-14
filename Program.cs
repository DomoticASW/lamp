var builder = WebApplication.CreateBuilder(args);

// Register controllers
builder.Services.AddControllers();

var app = builder.Build();

// Map controller routes
app.MapControllers();

app.Run();