using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCardCollection.Migrations
{
    public partial class extenddeckmodelbydescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Decks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Decks");
        }
    }
}
