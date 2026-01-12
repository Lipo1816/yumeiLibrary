using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.EFModel
{
    public class Data_Person_Permissions
    {
        [Key]
        public int Id { get; set; } // 主鍵

        [Required]
        public string 人員ID { get; set; } // 替換群組為人員ID

        public string 工單看板 { get; set; } = "0"; // 預設值為 "0"
        public string 設備看板 { get; set; } = "0";
        public string 品管看板 { get; set; } = "0";
        public string 設備管理 { get; set; } = "0";
        public string 設備點檢 { get; set; } = "0";
        public string 工單報工 { get; set; } = "0";
        public string 人員 { get; set; } = "0";
        public string 資料分析 { get; set; } = "0";
        public string 資料設定 { get; set; } = "0";
        public string 管理設定 { get; set; } = "0";
        public string 人員姓名 { get; set; } = string.Empty;
        public string 生產組名 { get; set; } = string.Empty;
    }
}
