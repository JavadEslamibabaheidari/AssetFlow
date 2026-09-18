using Inventory.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Infrastructure.Persistence;

public sealed class InventoryDbContext(DbContextOptions<InventoryDbContext> options)
    : DbContext(options)
{
    public DbSet<Vendor> Vendors => Set<Vendor>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<SalesChannel> Channels => Set<SalesChannel>();

    public DbSet<StockItem> StockItems => Set<StockItem>();

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
    }
}
