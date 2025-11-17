using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class AddTagLimitAlarmLogsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TagLimitAlarmLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MachineCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MachineName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TagName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TagDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CurrentValue = table.Column<double>(type: "float", nullable: true),
                    UpperLimit = table.Column<double>(type: "float", nullable: true),
                    LowerLimit = table.Column<double>(type: "float", nullable: true),
                    AlarmType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AlarmTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AlarmStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ResolvedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagLimitAlarmLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagLimitAlarmLogs");
        }
    }
}
