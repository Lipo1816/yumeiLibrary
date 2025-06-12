using System;
using DevExpress.Blazor.Internal.ComponentStructureHelpers;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommonLibraryP.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipmentSpecs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    項目 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    機台名稱 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    機種代碼 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    機台編號 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    線別編號 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    資訊項目 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    機台項目說明 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    機台項目代碼 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    規格型號 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    說明1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PLC讀值型態 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PLC_XY位址 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PLC讀值位址ModbusAdd = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    條件或格式 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    電控制箱編號 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    電控制箱IP = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentSpecs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InspectionLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    機台編號 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    機台名稱 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    產生時間 = table.Column<DateTime>(type: "datetime2", nullable: false),
                    完成時間 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    檢查人 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TYPE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    表單狀態 = table.Column<int>(type: "int", nullable: false),
                    單號 = table.Column<string>(type: "nvarchar(50)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InspectionRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    機台編號 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    機台名稱 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    項目 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    紀錄值 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    產生時間 = table.Column<DateTime>(type: "datetime2", nullable: false),
                    完成時間 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    檢查人 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    表單狀態 = table.Column<int>(type: "int", nullable: false),
                    檢查單號 = table.Column<string>(type: "nvarchar(50)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InspectionReportTimes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DailyTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    WeeklyDay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WeeklyTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    MonthlyDay = table.Column<int>(type: "int", nullable: true),
                    MonthlyTime = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionReportTimes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Inspections",
                columns: table => new
                {
                    機台編號 = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    項目 = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    機台名稱 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    點檢位置 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    頻率 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    檢查 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    標準 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    方式 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    紀錄值 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspections", x => new { x.機台編號, x.項目 });
                });

            migrationBuilder.CreateTable(
                name: "MachineStatusLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MachineID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LogTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineStatusLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModbusSlaveConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false),
                    Station = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModbusSlaveConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Personnal",
                columns: table => new
                {
                    人員ID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    部門ID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    部門名稱 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    生產組名 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    人員姓名 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    職級代號 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    職級名稱 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    權限 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    權限頁面 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personnal", x => x.人員ID);
                });

            migrationBuilder.CreateTable(
                name: "TagCategory",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConnectionType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagCategory", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "WorkorderChecks",
                columns: table => new
                {
                    工單號 = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    料號 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    品名 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    訂單號 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    工單發料量 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    生產組別 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    生產線別 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    客戶編號 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    排產日 = table.Column<DateTime>(type: "datetime2", nullable: false),
                    出貨日 = table.Column<DateTime>(type: "datetime2", nullable: false),
                    分盒數 = table.Column<int>(type: "int", nullable: false),
                    分盒總重量 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    製程程式 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    標準工時 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    發料儲位 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    物料採購單1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    物料採購單2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    物料採購單3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    工單計算方式 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    產品生產SOP1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    產品生產SOP2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    產品生產SOP3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    產品生產SOP4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    產品生產SOP5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    生產輔具1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    生產輔具2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    生產輔具3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    生產輔具4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    生產輔具5 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkorderChecks", x => x.工單號);
                });

            migrationBuilder.CreateTable(
                name: "Workorders",
                columns: table => new
                {
                    工單號 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    料號 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    品名 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    訂單號 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    工單發料量 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    生產組別 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    生產線別 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    客戶編號 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    排產日 = table.Column<DateTime>(type: "datetime2", nullable: false),
                    出貨日 = table.Column<DateTime>(type: "datetime2", nullable: false),
                    分盒數 = table.Column<int>(type: "int", nullable: false),
                    分盒總重量 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    製程程式 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    標準工時 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    發料儲位 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    物料採購單1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    物料採購單2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    物料採購單3 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    工單計算方式 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workorders", x => x.工單號);
                });

            migrationBuilder.CreateTable(
                name: "Machine",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false),
                    ConnectionType = table.Column<int>(type: "int", nullable: false),
                    MaxRetryCount = table.Column<int>(type: "int", nullable: false),
                    TagCategoryID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    UpdateDelay = table.Column<int>(type: "int", nullable: false),
                    RecordStatusChanged = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machine", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Machine_TagCategory_TagCategoryID",
                        column: x => x.TagCategoryID,
                        principalTable: "TagCategory",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ModbusTCPTags",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    UpdateByTime = table.Column<bool>(type: "bit", nullable: false),
                    Station = table.Column<byte>(type: "tinyint", nullable: false),
                    InputOrOutput = table.Column<bool>(type: "bit", nullable: false),
                    StartIndex = table.Column<int>(type: "int", nullable: false),
                    Offset = table.Column<int>(type: "int", nullable: false),
                    StringReverse = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModbusTCPTags", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ModbusTCPTags_TagCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "TagCategory",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Machine_Name",
                table: "Machine",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Machine_TagCategoryID",
                table: "Machine",
                column: "TagCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_ModbusTCPTags_CategoryId",
                table: "ModbusTCPTags",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ModbusTCPTags_Name",
                table: "ModbusTCPTags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TagCategory_Name",
                table: "TagCategory",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentSpecs");

            migrationBuilder.DropTable(
                name: "InspectionLists");

            migrationBuilder.DropTable(
                name: "InspectionRecords");

            migrationBuilder.DropTable(
                name: "InspectionReportTimes");

            migrationBuilder.DropTable(
                name: "Inspections");

            migrationBuilder.DropTable(
                name: "Machine");

            migrationBuilder.DropTable(
                name: "MachineStatusLogs");

            migrationBuilder.DropTable(
                name: "ModbusSlaveConfigs");

            migrationBuilder.DropTable(
                name: "ModbusTCPTags");

            migrationBuilder.DropTable(
                name: "Personnal");

            migrationBuilder.DropTable(
                name: "WorkorderChecks");

            migrationBuilder.DropTable(
                name: "Workorders");

            migrationBuilder.DropTable(
                name: "TagCategory");
        }
    }
}
