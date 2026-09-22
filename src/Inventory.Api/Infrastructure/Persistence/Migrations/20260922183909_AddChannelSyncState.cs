using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddChannelSyncState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "channel_sync_states",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    stock_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    available_quantity = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    attempt_count = table.Column<int>(type: "integer", nullable: false),
                    last_attempted_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    next_attempt_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_succeeded_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_channel_sync_states", x => x.id);
                    table.CheckConstraint("ck_channel_sync_states_attempt_count_non_negative", "attempt_count >= 0");
                    table.CheckConstraint("ck_channel_sync_states_status_valid", "status IN ('Pending', 'InProgress', 'Succeeded', 'Failed')");
                    table.ForeignKey(
                        name: "FK_channel_sync_states_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_channel_sync_states_stock_items_stock_item_id",
                        column: x => x.stock_item_id,
                        principalTable: "stock_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_channel_sync_states_channel_id_stock_item_id",
                table: "channel_sync_states",
                columns: new[] { "channel_id", "stock_item_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_channel_sync_states_status_next_attempt_at_utc",
                table: "channel_sync_states",
                columns: new[] { "status", "next_attempt_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_channel_sync_states_stock_item_id",
                table: "channel_sync_states",
                column: "stock_item_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "channel_sync_states");
        }
    }
}
