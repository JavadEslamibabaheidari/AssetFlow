using Inventory.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddInventoryPersistence(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new
{
    service = "Inventory API",
    message = "Marketplace inventory platform is running."
}))
.WithName("GetServiceInfo");

app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    service = "Inventory API",
    checkedAtUtc = DateTimeOffset.UtcNow
}))
.WithName("GetHealth");

app.Run();

public partial class Program;
