using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCardCollection.Migrations
{
    public partial class extendedappuserbyoverallstatistics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DecksCreated",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalCards",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "TotalValue",
                table: "AspNetUsers",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "UniqueCards",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DecksCreated",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TotalCards",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TotalValue",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UniqueCards",
                table: "AspNetUsers");
        }
    }
}
