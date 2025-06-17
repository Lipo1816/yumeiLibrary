using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEquipmentSpecLimit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "頻率上限",
                table: "EquipmentSpecLimits",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "頻率下限",
                table: "EquipmentSpecLimits",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "頻率上限",
                table: "EquipmentSpecLimits");

            migrationBuilder.DropColumn(
                name: "頻率下限",
                table: "EquipmentSpecLimits");
        }
    }
}
