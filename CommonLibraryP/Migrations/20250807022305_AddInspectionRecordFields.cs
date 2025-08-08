using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class AddInspectionRecordFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "方式",
                table: "InspectionRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "標準",
                table: "InspectionRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "檢查",
                table: "InspectionRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "結果",
                table: "InspectionRecords",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "方式",
                table: "InspectionRecords");

            migrationBuilder.DropColumn(
                name: "標準",
                table: "InspectionRecords");

            migrationBuilder.DropColumn(
                name: "檢查",
                table: "InspectionRecords");

            migrationBuilder.DropColumn(
                name: "結果",
                table: "InspectionRecords");
        }
    }
}
