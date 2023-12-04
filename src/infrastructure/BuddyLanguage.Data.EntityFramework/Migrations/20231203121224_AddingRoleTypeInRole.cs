using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuddyLanguage.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddingRoleTypeInRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Translation",
                table: "WordEntities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoleType",
                table: "Roles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleType",
                table: "Roles");

            migrationBuilder.AlterColumn<string>(
                name: "Translation",
                table: "WordEntities",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
