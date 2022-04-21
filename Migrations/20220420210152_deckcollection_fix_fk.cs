using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCardCollection.Migrations
{
    public partial class deckcollection_fix_fk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeckName",
                table: "DecksCollections");

            migrationBuilder.AddColumn<int>(
                name: "DeckId",
                table: "DecksCollections",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DecksCollections_DeckId",
                table: "DecksCollections",
                column: "DeckId");

            migrationBuilder.AddForeignKey(
                name: "FK_DecksCollections_Decks_DeckId",
                table: "DecksCollections",
                column: "DeckId",
                principalTable: "Decks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DecksCollections_Decks_DeckId",
                table: "DecksCollections");

            migrationBuilder.DropIndex(
                name: "IX_DecksCollections_DeckId",
                table: "DecksCollections");

            migrationBuilder.DropColumn(
                name: "DeckId",
                table: "DecksCollections");

            migrationBuilder.AddColumn<string>(
                name: "DeckName",
                table: "DecksCollections",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
