using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class AddInspectionRecordFieldnewthree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "檢查錯誤",
                table: "InspectionRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "檢查點位",
                table: "InspectionRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "維修期限",
                table: "InspectionRecords",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "檢查錯誤",
                table: "InspectionRecords");

            migrationBuilder.DropColumn(
                name: "檢查點位",
                table: "InspectionRecords");

            migrationBuilder.DropColumn(
                name: "維修期限",
                table: "InspectionRecords");
        }
    }
}
