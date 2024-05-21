using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace todo_api_app.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Users Table
            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "users",
                nullable: false,
                oldClrType: typeof(int),
                oldDefaultValue: null);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "users",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "users",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "created_by",
                table: "users",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "updated_by",
                table: "users",
                nullable: true);



            //User Token table
            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "user_token",
                nullable: false,
                oldClrType: typeof(int),
                oldDefaultValue: null);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "user_token",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "user_token",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "created_by",
                table: "user_token",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "updated_by",
                table: "user_token",
                nullable: true);



            //Todo table
            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "todo",
                nullable: false,
                oldClrType: typeof(int),
                oldDefaultValue: null);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "todo",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "todo",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "created_by",
                table: "todo",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "updated_by",
                table: "todo",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //Users Table
            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "users",
                nullable: false,
                oldClrType: typeof(int),
                oldDefaultValue: null);

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "users");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "users");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "users");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "users");



            //User Token table
            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "user_token",
                nullable: false,
                oldClrType: typeof(int),
                oldDefaultValue: null);

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "user_token");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "user_token");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "user_token");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "user_token");



            //Todo table
            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "todo",
                nullable: false,
                oldClrType: typeof(int),
                oldDefaultValue: null);

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "todo");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "todo");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "todo");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "todo");
        }
    }
}
