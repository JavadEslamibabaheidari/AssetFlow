using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationPersistence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "reservations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    stock_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    expires_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    released_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    expired_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservations", x => x.id);
                    table.CheckConstraint("ck_reservations_expired_at_matches_status", "(status = 'Expired' AND expired_at_utc IS NOT NULL) OR (status <> 'Expired' AND expired_at_utc IS NULL)");
                    table.CheckConstraint("ck_reservations_expires_after_created", "expires_at_utc > created_at_utc");
                    table.CheckConstraint("ck_reservations_quantity_positive", "quantity > 0");
                    table.CheckConstraint("ck_reservations_released_at_matches_status", "(status = 'Released' AND released_at_utc IS NOT NULL) OR (status <> 'Released' AND released_at_utc IS NULL)");
                    table.CheckConstraint("ck_reservations_status_valid", "status IN ('Active', 'Released', 'Expired')");
                    table.ForeignKey(
                        name: "FK_reservations_stock_items_stock_item_id",
                        column: x => x.stock_item_id,
                        principalTable: "stock_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_reservations_status_expires_at_utc",
                table: "reservations",
                columns: new[] { "status", "expires_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_reservations_stock_item_id",
                table: "reservations",
                column: "stock_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservations_stock_item_id_status_expires_at_utc",
                table: "reservations",
                columns: new[] { "stock_item_id", "status", "expires_at_utc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reservations");
        }
    }
}
