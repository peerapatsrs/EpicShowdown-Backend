using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EpicShowdown.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedGiftFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Points",
                table: "Gifts");

            migrationBuilder.DropColumn(
                name: "PricePerPiece",
                table: "Gifts");

            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "Gifts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "Gifts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerPiece",
                table: "Gifts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                table: "Gifts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
