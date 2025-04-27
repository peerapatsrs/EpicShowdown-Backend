using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EpicShowdown.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPassKeySupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Contests",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Contests",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTime>(
                name: "GiftEndDate",
                table: "Contests",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GiftStartDate",
                table: "Contests",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContestantGifts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContestantId = table.Column<int>(type: "integer", nullable: false),
                    ContestGiftId = table.Column<int>(type: "integer", nullable: false),
                    GivenById = table.Column<int>(type: "integer", nullable: true),
                    GivenAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContestantGifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContestantGifts_ContestGifts_ContestGiftId",
                        column: x => x.ContestGiftId,
                        principalTable: "ContestGifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContestantGifts_Contestants_ContestantId",
                        column: x => x.ContestantId,
                        principalTable: "Contestants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContestantGifts_Users_GivenById",
                        column: x => x.GivenById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContestantGifts_ContestantId",
                table: "ContestantGifts",
                column: "ContestantId");

            migrationBuilder.CreateIndex(
                name: "IX_ContestantGifts_ContestGiftId",
                table: "ContestantGifts",
                column: "ContestGiftId");

            migrationBuilder.CreateIndex(
                name: "IX_ContestantGifts_GivenById",
                table: "ContestantGifts",
                column: "GivenById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContestantGifts");

            migrationBuilder.DropColumn(
                name: "GiftEndDate",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "GiftStartDate",
                table: "Contests");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Contests",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Contests",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");
        }
    }
}
