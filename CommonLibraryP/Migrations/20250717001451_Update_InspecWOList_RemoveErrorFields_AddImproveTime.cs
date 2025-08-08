using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class Update_InspecWOList_RemoveErrorFields_AddImproveTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "錯誤描述",
                table: "InspecWOLists");

            migrationBuilder.DropColumn(
                name: "錯誤項目1",
                table: "InspecWOLists");

            migrationBuilder.DropColumn(
                name: "錯誤項目2",
                table: "InspecWOLists");

            migrationBuilder.DropColumn(
                name: "錯誤項目3",
                table: "InspecWOLists");

            migrationBuilder.DropColumn(
                name: "錯誤項目4",
                table: "InspecWOLists");

            migrationBuilder.AddColumn<DateTime>(
                name: "改善時間",
                table: "InspecWOLists",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "BreakTimeSchedules",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "BreakTimeSchedules",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "改善時間",
                table: "InspecWOLists");

            migrationBuilder.AddColumn<string>(
                name: "錯誤描述",
                table: "InspecWOLists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "錯誤項目1",
                table: "InspecWOLists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "錯誤項目2",
                table: "InspecWOLists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "錯誤項目3",
                table: "InspecWOLists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "錯誤項目4",
                table: "InspecWOLists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "BreakTimeSchedules",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "BreakTimeSchedules",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
