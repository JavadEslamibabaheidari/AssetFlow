using Inventory.Api.Api.ChannelSync;
using Inventory.Api.Api.Channels;
using Inventory.Api.Api.Observability;
using Inventory.Api.Api.Products;
using Inventory.Api.Api.Reservations;
using Inventory.Api.Api.StockItems;
using Inventory.Api.Api.Vendors;
using Inventory.Api.Infrastructure;
using Inventory.Api.Infrastructure.Observability;
using Inventory.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "AssetFlowFrontend";

builder.Services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddInventoryPersistence(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173",
                "http://localhost:4173",
                "http://127.0.0.1:4173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Configuration.GetValue<bool>("AssetFlow:ApplyDatabaseMigrations"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseCors(FrontendCorsPolicy);
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
