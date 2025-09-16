using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailSentSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailSentSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    人員ID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ischoose = table.Column<bool>(type: "bit", nullable: false),
                    建立時間 = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailSentSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailSentSettings");
        }
    }
}
