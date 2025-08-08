using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class AddBreakTimeSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BreakTimeSchedules",
                columns: table => new
                {
                    LineName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WeekDay = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PeriodNo = table.Column<int>(type: "int", nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsEnable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BreakTimeSchedules", x => new { x.LineName, x.WeekDay, x.PeriodNo, x.ModifyTime });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BreakTimeSchedules");
        }
    }
}
