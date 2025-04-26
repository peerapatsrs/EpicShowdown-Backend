using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EpicShowdown.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGiftCodeToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, add a new UUID column
            migrationBuilder.AddColumn<Guid>(
                name: "NewCode",
                table: "Gifts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid());

            // Convert existing string codes to UUID
            migrationBuilder.Sql(@"
                UPDATE ""Gifts""
                SET ""NewCode"" = CAST(""Code"" AS uuid)
                WHERE ""Code"" ~ '^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$'
            ");

            // Drop the old column
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Gifts");

            // Rename the new column to Code
            migrationBuilder.RenameColumn(
                name: "NewCode",
                table: "Gifts",
                newName: "Code");

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "Gifts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add old string column
            migrationBuilder.AddColumn<string>(
                name: "OldCode",
                table: "Gifts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            // Convert UUID back to string
            migrationBuilder.Sql(@"
                UPDATE ""Gifts""
                SET ""OldCode"" = ""Code""::text
            ");

            // Drop the UUID column
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Gifts");

            // Rename old column back to Code
            migrationBuilder.RenameColumn(
                name: "OldCode",
                table: "Gifts",
                newName: "Code");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "Gifts");
        }
    }
}
