using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EpicShowdown.API.Migrations
{
    /// <inheritdoc />
    public partial class UniqueCodeAll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gifts_Code",
                table: "Gifts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contests_ContestCode",
                table: "Contests",
                column: "ContestCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Gifts_Code",
                table: "Gifts");

            migrationBuilder.DropIndex(
                name: "IX_Contests_ContestCode",
                table: "Contests");
        }
    }
}
