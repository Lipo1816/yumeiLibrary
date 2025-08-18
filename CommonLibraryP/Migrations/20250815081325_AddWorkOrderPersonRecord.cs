using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkOrderPersonRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkOrderPersonRecords",
                columns: table => new
                {
                    姓名 = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    工單 = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    時間 = table.Column<DateTime>(type: "datetime2", nullable: false),
                    狀態 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderPersonRecords", x => new { x.姓名, x.工單, x.時間 });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkOrderPersonRecords");
        }
    }
}
