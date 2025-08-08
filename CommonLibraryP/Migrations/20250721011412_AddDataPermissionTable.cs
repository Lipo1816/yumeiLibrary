using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class AddDataPermissionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Data_Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    群組 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    工單看板 = table.Column<bool>(type: "bit", nullable: false),
                    設備看板 = table.Column<bool>(type: "bit", nullable: false),
                    品管看板 = table.Column<bool>(type: "bit", nullable: false),
                    設備管理 = table.Column<bool>(type: "bit", nullable: false),
                    設備點檢 = table.Column<bool>(type: "bit", nullable: false),
                    工單報工 = table.Column<bool>(type: "bit", nullable: false),
                    人員 = table.Column<bool>(type: "bit", nullable: false),
                    資料分析 = table.Column<bool>(type: "bit", nullable: false),
                    資料設定 = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Data_Permissions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Data_Permissions");
        }
    }
}
