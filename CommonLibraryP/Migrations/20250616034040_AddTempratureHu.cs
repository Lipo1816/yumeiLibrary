using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class AddTempratureHu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "temprature_Hu_logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MachineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MachineNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MachineGroupNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    temperature = table.Column<double>(type: "float", nullable: true),
                    humidity = table.Column<double>(type: "float", nullable: true),
                    battery = table.Column<double>(type: "float", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_temprature_Hu_logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "temprature_Hus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MachineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MachineNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MachineGroupNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    temperature_high = table.Column<double>(type: "float", nullable: true),
                    temperature_low = table.Column<double>(type: "float", nullable: true),
                    humidity_high = table.Column<double>(type: "float", nullable: true),
                    humidity_low = table.Column<double>(type: "float", nullable: true),
                    battery_high = table.Column<double>(type: "float", nullable: true),
                    battery_low = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_temprature_Hus", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "temprature_Hu_logs");

            migrationBuilder.DropTable(
                name: "temprature_Hus");
        }
    }
}
