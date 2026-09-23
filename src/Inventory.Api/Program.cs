using Inventory.Api.Api.ChannelSync;
using Inventory.Api.Api.Channels;
using Inventory.Api.Api.Observability;
using Inventory.Api.Api.Products;
using Inventory.Api.Api.Reservations;
using Inventory.Api.Api.StockItems;
using Inventory.Api.Api.Vendors;
using Inventory.Api.Infrastructure;
using Inventory.Api.Infrastructure.Observability;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddInventoryPersistence(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<RequestObservabilityMiddleware>();

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

app.MapVendorEndpoints();
app.MapProductEndpoints();
app.MapChannelEndpoints();
app.MapChannelSyncEndpoints();
app.MapObservabilityEndpoints();
app.MapStockItemEndpoints();
app.MapReservationEndpoints();

app.Run();

public partial class Program;
