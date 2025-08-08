using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInspecWOListTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NG數量",
                table: "InspecWOLists",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "備註",
                table: "InspecWOLists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "分類",
                table: "InspecWOLists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "檢查數量",
                table: "InspecWOLists",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "責任單位",
                table: "InspecWOLists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "錯誤代碼",
                table: "InspecWOLists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "錯誤項目",
                table: "InspecWOLists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "點檢內容",
                table: "Inspection_WoItem",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "錯誤代碼",
                table: "Inspection_WoItem",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "分類",
                table: "Inspection_WoItem",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Index",
                table: "Inspection_WoItem",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NG數量",
                table: "InspecWOLists");

            migrationBuilder.DropColumn(
                name: "備註",
                table: "InspecWOLists");

            migrationBuilder.DropColumn(
                name: "分類",
                table: "InspecWOLists");

            migrationBuilder.DropColumn(
                name: "檢查數量",
                table: "InspecWOLists");

            migrationBuilder.DropColumn(
                name: "責任單位",
                table: "InspecWOLists");

            migrationBuilder.DropColumn(
                name: "錯誤代碼",
                table: "InspecWOLists");

            migrationBuilder.DropColumn(
                name: "錯誤項目",
                table: "InspecWOLists");

            migrationBuilder.DropColumn(
                name: "Index",
                table: "Inspection_WoItem");

            migrationBuilder.AlterColumn<string>(
                name: "點檢內容",
                table: "Inspection_WoItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "錯誤代碼",
                table: "Inspection_WoItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "分類",
                table: "Inspection_WoItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
