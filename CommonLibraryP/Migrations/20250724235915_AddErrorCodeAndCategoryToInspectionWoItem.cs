using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class AddErrorCodeAndCategoryToInspectionWoItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "分類",
                table: "Inspection_WoItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "錯誤代碼",
                table: "Inspection_WoItem",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "分類",
                table: "Inspection_WoItem");

            migrationBuilder.DropColumn(
                name: "錯誤代碼",
                table: "Inspection_WoItem");
        }
    }
}
