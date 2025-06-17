using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentSpecLimit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipmentSpecLimits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    項目 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    機台名稱 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    機種代碼 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    機台編號 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    線別編號 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    資訊項目 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    機台項目說明 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    機台項目代碼 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    規格型號 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    說明1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    電控制箱編號 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    電控制箱IP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    電壓上限 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    電壓下限 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    電流上限 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    電流下限 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    轉速上限 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    轉速下限 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    水溫上限 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    水溫下限 = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentSpecLimits", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentSpecLimits");
        }
    }
}
