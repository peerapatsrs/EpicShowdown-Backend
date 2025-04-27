using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EpicShowdown.API.Migrations
{
    /// <inheritdoc />
    public partial class FixPassKey01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PassKeys");

            migrationBuilder.AddColumn<Guid>(
                name: "UserCode",
                table: "PassKeys",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserCode",
                table: "PassKeys");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "PassKeys",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
