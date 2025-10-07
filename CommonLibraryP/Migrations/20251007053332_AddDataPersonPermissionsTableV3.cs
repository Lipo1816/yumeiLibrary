using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class AddDataPersonPermissionsTableV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataPersonPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    人員ID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    工單看板 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    設備看板 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    品管看板 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    設備管理 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    設備點檢 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    工單報工 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    人員 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    資料分析 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    資料設定 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataPersonPermissions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataPersonPermissions");
        }
    }
}
