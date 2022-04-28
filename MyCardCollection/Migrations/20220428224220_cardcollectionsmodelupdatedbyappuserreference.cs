using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCardCollection.Migrations
{
    public partial class cardcollectionsmodelupdatedbyappuserreference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Collection_AspNetUsers_AppUserId",
                table: "Collection");

            migrationBuilder.DropIndex(
                name: "IX_Collection_AppUserId",
                table: "Collection");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Collection");

            migrationBuilder.CreateIndex(
                name: "IX_Collection_UserId",
                table: "Collection",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Collection_AspNetUsers_UserId",
                table: "Collection",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Collection_AspNetUsers_UserId",
                table: "Collection");

            migrationBuilder.DropIndex(
                name: "IX_Collection_UserId",
                table: "Collection");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Collection",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Collection_AppUserId",
                table: "Collection",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Collection_AspNetUsers_AppUserId",
                table: "Collection",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
