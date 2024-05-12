using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace todo_api_app.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_user_token_UserTokenId",
                table: "users");

            migrationBuilder.AlterColumn<int>(
                name: "UserTokenId",
                table: "users",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_users_user_token_UserTokenId",
                table: "users",
                column: "UserTokenId",
                principalTable: "user_token",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_user_token_UserTokenId",
                table: "users");

            migrationBuilder.AlterColumn<int>(
                name: "UserTokenId",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_users_user_token_UserTokenId",
                table: "users",
                column: "UserTokenId",
                principalTable: "user_token",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
