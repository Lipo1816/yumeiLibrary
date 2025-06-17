using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class AddEquiManufacturerInformation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquiManufacturer_Information",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PLC項目 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    機台名稱 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    機種代碼 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    機台編號 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    線別編號 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    電控箱 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    購入日期 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    設備廠商 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    廠商電話 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    廠商MAIL = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquiManufacturer_Information", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquiManufacturer_Information");
        }
    }
}
