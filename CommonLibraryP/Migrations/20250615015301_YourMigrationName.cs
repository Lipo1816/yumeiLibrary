using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class YourMigrationName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuarterDay",
                table: "InspectionReportTimes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuarterHour",
                table: "InspectionReportTimes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuarterMonth",
                table: "InspectionReportTimes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearDay",
                table: "InspectionReportTimes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearHour",
                table: "InspectionReportTimes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearMonth",
                table: "InspectionReportTimes",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuarterDay",
                table: "InspectionReportTimes");

            migrationBuilder.DropColumn(
                name: "QuarterHour",
                table: "InspectionReportTimes");

            migrationBuilder.DropColumn(
                name: "QuarterMonth",
                table: "InspectionReportTimes");

            migrationBuilder.DropColumn(
                name: "YearDay",
                table: "InspectionReportTimes");

            migrationBuilder.DropColumn(
                name: "YearHour",
                table: "InspectionReportTimes");

            migrationBuilder.DropColumn(
                name: "YearMonth",
                table: "InspectionReportTimes");
        }
    }
}
