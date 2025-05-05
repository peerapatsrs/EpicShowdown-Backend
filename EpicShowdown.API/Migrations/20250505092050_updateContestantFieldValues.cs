using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EpicShowdown.API.Migrations
{
    /// <inheritdoc />
    public partial class updateContestantFieldValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContestantGifts_Contestants_ContestantId",
                table: "ContestantGifts");

            migrationBuilder.DropTable(
                name: "Contestants");

            migrationBuilder.CreateTable(
                name: "ContestContestants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContestId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContestContestants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContestContestants_Contests_ContestId",
                        column: x => x.ContestId,
                        principalTable: "Contests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContestantFieldValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContestContestantId = table.Column<int>(type: "integer", nullable: true),
                    FieldName = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContestantFieldValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContestantFieldValues_ContestContestants_ContestContestantId",
                        column: x => x.ContestContestantId,
                        principalTable: "ContestContestants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContestantFieldValues_ContestContestantId",
                table: "ContestantFieldValues",
                column: "ContestContestantId");

            migrationBuilder.CreateIndex(
                name: "IX_ContestContestants_ContestId",
                table: "ContestContestants",
                column: "ContestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContestantGifts_ContestContestants_ContestantId",
                table: "ContestantGifts",
                column: "ContestantId",
                principalTable: "ContestContestants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContestantGifts_ContestContestants_ContestantId",
                table: "ContestantGifts");

            migrationBuilder.DropTable(
                name: "ContestantFieldValues");

            migrationBuilder.DropTable(
                name: "ContestContestants");

            migrationBuilder.CreateTable(
                name: "Contestants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContestId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FieldName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contestants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contestants_Contests_ContestId",
                        column: x => x.ContestId,
                        principalTable: "Contests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contestants_ContestId",
                table: "Contestants",
                column: "ContestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContestantGifts_Contestants_ContestantId",
                table: "ContestantGifts",
                column: "ContestantId",
                principalTable: "Contestants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
