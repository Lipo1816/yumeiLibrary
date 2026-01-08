using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class AddIncompleteCategoryDescriptionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "未完成說明",
                table: "InspectionRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "未完成類別",
                table: "InspectionRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "維修完成日",
                table: "InspectionRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IncompleteCategoryDescriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    未完成類別 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    未完成說明 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    排序順序 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncompleteCategoryDescriptions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncompleteCategoryDescriptions");

            migrationBuilder.DropColumn(
                name: "未完成說明",
                table: "InspectionRecords");

            migrationBuilder.DropColumn(
                name: "未完成類別",
                table: "InspectionRecords");

            migrationBuilder.DropColumn(
                name: "維修完成日",
                table: "InspectionRecords");
        }
    }
}
