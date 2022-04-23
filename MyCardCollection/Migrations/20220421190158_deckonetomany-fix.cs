using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCardCollection.Migrations
{
    public partial class deckonetomanyfix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Collection_Decks_DeckId",
                table: "Collection");

            migrationBuilder.DropIndex(
                name: "IX_Collection_DeckId",
                table: "Collection");

            migrationBuilder.DropColumn(
                name: "DeckId",
                table: "Collection");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeckId",
                table: "Collection",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Collection_DeckId",
                table: "Collection",
                column: "DeckId");

            migrationBuilder.AddForeignKey(
                name: "FK_Collection_Decks_DeckId",
                table: "Collection",
                column: "DeckId",
                principalTable: "Decks",
                principalColumn: "Id");
        }
    }
}
