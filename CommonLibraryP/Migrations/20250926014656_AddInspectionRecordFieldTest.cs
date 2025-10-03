using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class AddInspectionRecordFieldTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Test",
                table: "InspectionRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Test",
                table: "InspectionRecords");
        }
    }
}
