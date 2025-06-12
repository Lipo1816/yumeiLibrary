using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class AddProblemDescriptTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProblemDescripts",
                columns: table => new
                {
                    不良代碼 = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    不良類型代碼 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    不良類型 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    不良描述 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemDescripts", x => x.不良代碼);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProblemDescripts");
        }
    }
}
