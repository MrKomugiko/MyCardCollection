using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCardCollection.Migrations
{
    public partial class changecardmodel_addcroppedimageurl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageURLCropped",
                table: "CardsDatabase",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Transform_ImageURLCropped",
                table: "CardsDatabase",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageURLCropped",
                table: "CardsDatabase");

            migrationBuilder.DropColumn(
                name: "Transform_ImageURLCropped",
                table: "CardsDatabase");
        }
    }
}
