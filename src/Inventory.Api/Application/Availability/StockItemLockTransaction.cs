using Inventory.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Inventory.Api.Application.Availability;

public sealed class StockItemLockTransaction : IAsyncDisposable
{
    private readonly IDbContextTransaction? _transaction;
    private bool _committed;

    private StockItemLockTransaction(IDbContextTransaction? transaction)
    {
        _transaction = transaction;
    }

    public static async Task<StockItemLockTransaction> BeginAsync(
        InventoryDbContext dbContext,
        IEnumerable<Guid> stockItemIds,
        CancellationToken cancellationToken)
    {
        if (dbContext.Database.ProviderName != "Npgsql.EntityFrameworkCore.PostgreSQL")
        {
            return new StockItemLockTransaction(transaction: null);
        }

        var ids = stockItemIds
            .Distinct()
            .OrderBy(id => id)
            .ToArray();

        var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        foreach (var stockItemId in ids)
        {
            await dbContext.Database.ExecuteSqlInterpolatedAsync(
                $"SELECT id FROM stock_items WHERE id = {stockItemId} FOR UPDATE",
                cancellationToken);
        }

        return new StockItemLockTransaction(transaction);
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        if (_transaction is null)
        {
            return;
        }

        await _transaction.CommitAsync(cancellationToken);
        _committed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction is null)
        {
            return;
        }

        if (!_committed)
        {
            await _transaction.RollbackAsync();
        }

        await _transaction.DisposeAsync();
    }
}
