using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmLibraryPP.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerToFilm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Films",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Films_OwnerId",
                table: "Films",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Films_AspNetUsers_OwnerId",
                table: "Films",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Films_AspNetUsers_OwnerId",
                table: "Films");

            migrationBuilder.DropIndex(
                name: "IX_Films_OwnerId",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Films");
        }
    }
}
