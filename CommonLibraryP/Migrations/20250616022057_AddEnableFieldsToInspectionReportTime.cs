using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class AddEnableFieldsToInspectionReportTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DailyEnable",
                table: "InspectionReportTimes",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MonthlyEnable",
                table: "InspectionReportTimes",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "QuarterEnable",
                table: "InspectionReportTimes",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WeeklyEnable",
                table: "InspectionReportTimes",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "YearEnable",
                table: "InspectionReportTimes",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyEnable",
                table: "InspectionReportTimes");

            migrationBuilder.DropColumn(
                name: "MonthlyEnable",
                table: "InspectionReportTimes");

            migrationBuilder.DropColumn(
                name: "QuarterEnable",
                table: "InspectionReportTimes");

            migrationBuilder.DropColumn(
                name: "WeeklyEnable",
                table: "InspectionReportTimes");

            migrationBuilder.DropColumn(
                name: "YearEnable",
                table: "InspectionReportTimes");
        }
    }
}
