using Inventory.Api.Domain.Entities;
using Inventory.Api.Infrastructure.ChannelSync;
using Inventory.Api.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Infrastructure.Persistence;

public sealed class InventoryDbContext(DbContextOptions<InventoryDbContext> options)
    : DbContext(options)
{
    public DbSet<Vendor> Vendors => Set<Vendor>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<SalesChannel> Channels => Set<SalesChannel>();

    public DbSet<StockItem> StockItems => Set<StockItem>();

    public DbSet<Reservation> Reservations => Set<Reservation>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public DbSet<ChannelSyncState> ChannelSyncStates => Set<ChannelSyncState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vendor>(builder =>
        {
            builder.ToTable("vendors");

            builder.HasKey(vendor => vendor.Id);

            builder.Property(vendor => vendor.Id)
                .HasColumnName("id");

            builder.Property(vendor => vendor.Name)
                .HasColumnName("name")
                .HasMaxLength(160)
                .IsRequired();

            builder.HasIndex(vendor => vendor.Name)
                .IsUnique();

            builder.Property(vendor => vendor.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();
        });

        modelBuilder.Entity<Product>(builder =>
        {
            builder.ToTable("products");

            builder.HasKey(product => product.Id);

            builder.Property(product => product.Id)
                .HasColumnName("id");

            builder.Property(product => product.VendorId)
                .HasColumnName("vendor_id");

            builder.Property(product => product.Sku)
                .HasColumnName("sku")
                .HasMaxLength(80)
                .IsRequired();

            builder.Property(product => product.Name)
                .HasColumnName("name")
                .HasMaxLength(240)
                .IsRequired();

            builder.Property(product => product.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();

            builder.HasIndex(product => new { product.VendorId, product.Sku })
                .IsUnique();

            builder.HasOne(product => product.Vendor)
                .WithMany()
                .HasForeignKey(product => product.VendorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SalesChannel>(builder =>
        {
            builder.ToTable("channels");

            builder.HasKey(channel => channel.Id);

            builder.Property(channel => channel.Id)
                .HasColumnName("id");

            builder.Property(channel => channel.Code)
                .HasColumnName("code")
                .HasMaxLength(80)
                .IsRequired();

            builder.Property(channel => channel.Name)
                .HasColumnName("name")
                .HasMaxLength(160)
                .IsRequired();

            builder.Property(channel => channel.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();

            builder.HasIndex(channel => channel.Code)
                .IsUnique();
        });

        modelBuilder.Entity<StockItem>(builder =>
        {
            builder.ToTable("stock_items");

            builder.HasKey(stockItem => stockItem.Id);

            builder.Property(stockItem => stockItem.Id)
                .HasColumnName("id");

            builder.Property(stockItem => stockItem.ProductId)
                .HasColumnName("product_id");

            builder.Property(stockItem => stockItem.ChannelId)
                .HasColumnName("channel_id");

            builder.Property(stockItem => stockItem.OnHandQuantity)
                .HasColumnName("on_hand_quantity")
                .IsRequired();

            builder.Property(stockItem => stockItem.AvailableQuantity)
                .HasColumnName("available_quantity")
                .IsRequired();

            builder.Property(stockItem => stockItem.UpdatedAtUtc)
                .HasColumnName("updated_at_utc")
                .IsRequired();

            builder.HasIndex(stockItem => new { stockItem.ProductId, stockItem.ChannelId })
                .IsUnique();

            builder.HasOne(stockItem => stockItem.Product)
                .WithMany()
                .HasForeignKey(stockItem => stockItem.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(stockItem => stockItem.Channel)
                .WithMany()
                .HasForeignKey(stockItem => stockItem.ChannelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable(table =>
            {
                table.HasCheckConstraint("ck_stock_items_on_hand_quantity_non_negative", "on_hand_quantity >= 0");
                table.HasCheckConstraint("ck_stock_items_available_quantity_non_negative", "available_quantity >= 0");
                table.HasCheckConstraint(
                    "ck_stock_items_available_quantity_not_above_on_hand",
                    "available_quantity <= on_hand_quantity");
            });
        });

        modelBuilder.Entity<Reservation>(builder =>
        {
            builder.ToTable("reservations");

            builder.HasKey(reservation => reservation.Id);

            builder.Property(reservation => reservation.Id)
                .HasColumnName("id");

            builder.Property(reservation => reservation.StockItemId)
                .HasColumnName("stock_item_id");

            builder.Property(reservation => reservation.Quantity)
                .HasColumnName("quantity")
                .IsRequired();

            builder.Property(reservation => reservation.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(reservation => reservation.ExpiresAtUtc)
                .HasColumnName("expires_at_utc")
                .IsRequired();

            builder.Property(reservation => reservation.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();

            builder.Property(reservation => reservation.UpdatedAtUtc)
                .HasColumnName("updated_at_utc")
                .IsRequired();

            builder.Property(reservation => reservation.ReleasedAtUtc)
                .HasColumnName("released_at_utc");

            builder.Property(reservation => reservation.ExpiredAtUtc)
                .HasColumnName("expired_at_utc");

            builder.HasIndex(reservation => reservation.StockItemId);

            builder.HasIndex(reservation => new { reservation.StockItemId, reservation.Status, reservation.ExpiresAtUtc });

            builder.HasIndex(reservation => new { reservation.Status, reservation.ExpiresAtUtc });

            builder.HasOne(reservation => reservation.StockItem)
                .WithMany()
                .HasForeignKey(reservation => reservation.StockItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable(table =>
            {
                table.HasCheckConstraint("ck_reservations_quantity_positive", "quantity > 0");
                table.HasCheckConstraint(
                    "ck_reservations_status_valid",
                    "status IN ('Active', 'Released', 'Expired')");
                table.HasCheckConstraint(
                    "ck_reservations_expires_after_created",
                    "expires_at_utc > created_at_utc");
                table.HasCheckConstraint(
                    "ck_reservations_released_at_matches_status",
                    "(status = 'Released' AND released_at_utc IS NOT NULL) OR (status <> 'Released' AND released_at_utc IS NULL)");
                table.HasCheckConstraint(
                    "ck_reservations_expired_at_matches_status",
                    "(status = 'Expired' AND expired_at_utc IS NOT NULL) OR (status <> 'Expired' AND expired_at_utc IS NULL)");
            });
        });

        modelBuilder.Entity<OutboxMessage>(builder =>
        {
            builder.ToTable("outbox_messages");

            builder.HasKey(message => message.Id);

            builder.Property(message => message.Id)
                .HasColumnName("id");

            builder.Property(message => message.EventType)
                .HasColumnName("event_type")
                .HasMaxLength(160)
                .IsRequired();

            builder.Property(message => message.SchemaVersion)
                .HasColumnName("schema_version")
                .IsRequired();

            builder.Property(message => message.AggregateType)
                .HasColumnName("aggregate_type")
                .HasMaxLength(80)
                .IsRequired();

            builder.Property(message => message.AggregateId)
                .HasColumnName("aggregate_id")
                .IsRequired();

            builder.Property(message => message.OccurredAtUtc)
                .HasColumnName("occurred_at_utc")
                .IsRequired();

            builder.Property(message => message.PayloadJson)
                .HasColumnName("payload_json")
                .HasColumnType("jsonb")
                .IsRequired();

            builder.Property(message => message.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(message => message.AttemptCount)
                .HasColumnName("attempt_count")
                .IsRequired();

            builder.Property(message => message.NextAttemptAtUtc)
                .HasColumnName("next_attempt_at_utc");

            builder.Property(message => message.LastError)
                .HasColumnName("last_error")
                .HasMaxLength(2000);

            builder.Property(message => message.ProcessedAtUtc)
                .HasColumnName("processed_at_utc");

            builder.Property(message => message.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();

            builder.HasIndex(message => new { message.Status, message.NextAttemptAtUtc, message.CreatedAtUtc });

            builder.HasIndex(message => new { message.AggregateType, message.AggregateId });

            builder.ToTable(table =>
            {
                table.HasCheckConstraint("ck_outbox_messages_schema_version_positive", "schema_version > 0");
                table.HasCheckConstraint("ck_outbox_messages_attempt_count_non_negative", "attempt_count >= 0");
                table.HasCheckConstraint(
                    "ck_outbox_messages_status_valid",
                    "status IN ('Pending', 'Processing', 'Published', 'Failed')");
            });
        });

        modelBuilder.Entity<ChannelSyncState>(builder =>
        {
            builder.ToTable("channel_sync_states");

            builder.HasKey(state => state.Id);

            builder.Property(state => state.Id)
                .HasColumnName("id");

            builder.Property(state => state.ChannelId)
                .HasColumnName("channel_id")
                .IsRequired();

            builder.Property(state => state.StockItemId)
                .HasColumnName("stock_item_id")
                .IsRequired();

            builder.Property(state => state.SourceEventId)
                .HasColumnName("source_event_id")
                .IsRequired();

            builder.Property(state => state.AvailableQuantity)
                .HasColumnName("available_quantity")
                .IsRequired();

            builder.Property(state => state.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(state => state.AttemptCount)
                .HasColumnName("attempt_count")
                .IsRequired();

            builder.Property(state => state.LastAttemptedAtUtc)
                .HasColumnName("last_attempted_at_utc");

            builder.Property(state => state.NextAttemptAtUtc)
                .HasColumnName("next_attempt_at_utc");

            builder.Property(state => state.LastSucceededAtUtc)
                .HasColumnName("last_succeeded_at_utc");

            builder.Property(state => state.LastError)
                .HasColumnName("last_error")
                .HasMaxLength(2000);

            builder.Property(state => state.UpdatedAtUtc)
                .HasColumnName("updated_at_utc")
                .IsRequired();

            builder.HasIndex(state => new { state.ChannelId, state.StockItemId })
                .IsUnique();

            builder.HasIndex(state => new { state.Status, state.NextAttemptAtUtc });

            builder.HasOne<SalesChannel>()
                .WithMany()
                .HasForeignKey(state => state.ChannelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<StockItem>()
                .WithMany()
                .HasForeignKey(state => state.StockItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable(table =>
            {
                table.HasCheckConstraint("ck_channel_sync_states_attempt_count_non_negative", "attempt_count >= 0");
                table.HasCheckConstraint(
                    "ck_channel_sync_states_status_valid",
                    "status IN ('Pending', 'InProgress', 'Succeeded', 'Failed')");
            });
        });
    }
}
