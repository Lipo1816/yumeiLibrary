using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class AddInspectionWoItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inspection_WoItem",
                columns: table => new
                {
                    點檢單號 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    點檢項目 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    點檢時間 = table.Column<DateTime>(type: "datetime2", nullable: false),
                    錯誤項目 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    備註 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    責任單位 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    結果 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspection_WoItem", x => new { x.點檢單號, x.點檢項目 });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inspection_WoItem");
        }
    }
}
