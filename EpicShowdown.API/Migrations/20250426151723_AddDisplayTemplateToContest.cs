using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EpicShowdown.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDisplayTemplateToContest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisplayTemplateId",
                table: "Contests",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DisplayTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisplayTemplate", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contests_DisplayTemplateId",
                table: "Contests",
                column: "DisplayTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contests_DisplayTemplate_DisplayTemplateId",
                table: "Contests",
                column: "DisplayTemplateId",
                principalTable: "DisplayTemplate",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contests_DisplayTemplate_DisplayTemplateId",
                table: "Contests");

            migrationBuilder.DropTable(
                name: "DisplayTemplate");

            migrationBuilder.DropIndex(
                name: "IX_Contests_DisplayTemplateId",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "DisplayTemplateId",
                table: "Contests");
        }
    }
}
