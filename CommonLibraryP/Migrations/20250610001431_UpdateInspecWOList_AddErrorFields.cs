using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInspecWOList_AddErrorFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "result",
                table: "InspecWOLists",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "錯誤描述",
                table: "InspecWOLists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "錯誤項目1",
                table: "InspecWOLists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "錯誤項目2",
                table: "InspecWOLists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "錯誤項目3",
                table: "InspecWOLists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "錯誤項目4",
                table: "InspecWOLists",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "result",
                table: "InspecWOLists");

            migrationBuilder.DropColumn(
                name: "錯誤描述",
                table: "InspecWOLists");

            migrationBuilder.DropColumn(
                name: "錯誤項目1",
                table: "InspecWOLists");

            migrationBuilder.DropColumn(
                name: "錯誤項目2",
                table: "InspecWOLists");

            migrationBuilder.DropColumn(
                name: "錯誤項目3",
                table: "InspecWOLists");

            migrationBuilder.DropColumn(
                name: "錯誤項目4",
                table: "InspecWOLists");
        }
    }
}
