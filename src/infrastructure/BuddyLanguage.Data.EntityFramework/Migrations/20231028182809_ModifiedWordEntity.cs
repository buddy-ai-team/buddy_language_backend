using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuddyLanguage.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedWordEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Word",
                table: "WordEntities",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "WordEntities",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "WordEntities");

            migrationBuilder.AlterColumn<string>(
                name: "Word",
                table: "WordEntities",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
