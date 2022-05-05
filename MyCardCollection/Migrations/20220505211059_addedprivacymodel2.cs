using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCardCollection.Migrations
{
    public partial class addedprivacymodel2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrivacySettings_AspNetUsers_UserId",
                table: "PrivacySettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PrivacySettings",
                table: "PrivacySettings");

            migrationBuilder.RenameTable(
                name: "PrivacySettings",
                newName: "UserPrivacySettings");

            migrationBuilder.RenameIndex(
                name: "IX_PrivacySettings_UserId",
                table: "UserPrivacySettings",
                newName: "IX_UserPrivacySettings_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPrivacySettings",
                table: "UserPrivacySettings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPrivacySettings_AspNetUsers_UserId",
                table: "UserPrivacySettings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPrivacySettings_AspNetUsers_UserId",
                table: "UserPrivacySettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPrivacySettings",
                table: "UserPrivacySettings");

            migrationBuilder.RenameTable(
                name: "UserPrivacySettings",
                newName: "PrivacySettings");

            migrationBuilder.RenameIndex(
                name: "IX_UserPrivacySettings_UserId",
                table: "PrivacySettings",
                newName: "IX_PrivacySettings_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PrivacySettings",
                table: "PrivacySettings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PrivacySettings_AspNetUsers_UserId",
                table: "PrivacySettings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
