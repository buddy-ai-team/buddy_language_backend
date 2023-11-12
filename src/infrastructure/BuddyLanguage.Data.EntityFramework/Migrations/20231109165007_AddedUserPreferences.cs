using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuddyLanguage.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserPreferences_NativeLanguage",
                table: "Users",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UserPreferences_SelectedSpeed",
                table: "Users",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UserPreferences_SelectedVoice",
                table: "Users",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UserPreferences_TargetLanguage",
                table: "Users",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserPreferences_NativeLanguage",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserPreferences_SelectedSpeed",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserPreferences_SelectedVoice",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserPreferences_TargetLanguage",
                table: "Users");
        }
    }
}
