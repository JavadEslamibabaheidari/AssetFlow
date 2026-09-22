using Inventory.Api.Application.Events;
using Inventory.Api.Infrastructure.Persistence;
using Inventory.Api.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Infrastructure;

public static class DependencyInjection
{
    private const string DefaultInventoryConnectionString =
        "Host=localhost;Port=5432;Database=assetflow_inventory;Username=assetflow;Password=assetflow";

    public static IServiceCollection AddInventoryPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("InventoryDb")
            ?? DefaultInventoryConnectionString;

        services.AddDbContext<InventoryDbContext>(options =>
            options.UseNpgsql(connectionString));
        services.AddScoped<IIntegrationEventOutbox, EfIntegrationEventOutbox>();

        return services;
    }
}
