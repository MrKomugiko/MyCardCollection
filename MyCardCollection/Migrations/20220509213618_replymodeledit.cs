using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCardCollection.Migrations
{
    public partial class replymodeledit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Replies_Comment_Replies_ReplyTo",
                table: "Comment_Replies");

            migrationBuilder.AlterColumn<int>(
                name: "ReplyTo",
                table: "Comment_Replies",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Replies_Comment_Replies_ReplyTo",
                table: "Comment_Replies",
                column: "ReplyTo",
                principalTable: "Comment_Replies",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Replies_Comment_Replies_ReplyTo",
                table: "Comment_Replies");

            migrationBuilder.AlterColumn<int>(
                name: "ReplyTo",
                table: "Comment_Replies",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Replies_Comment_Replies_ReplyTo",
                table: "Comment_Replies",
                column: "ReplyTo",
                principalTable: "Comment_Replies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
