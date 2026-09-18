using Inventory.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Inventory.Api.Tests;

public sealed class TestInventoryApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"inventory-api-tests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<InventoryDbContext>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<DbContextOptions<InventoryDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<InventoryDbContext>>();
            services.RemoveAll<IDatabaseProvider>();

            services.AddDbContext<InventoryDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }
}
