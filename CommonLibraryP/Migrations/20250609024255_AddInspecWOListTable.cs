using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class AddInspecWOListTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InspecWOLists",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    工單 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    點檢單號 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    狀態 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    報工時間 = table.Column<DateTime>(type: "datetime2", nullable: false),
                    報工人員 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspecWOLists", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InspecWOLists");
        }
    }
}
