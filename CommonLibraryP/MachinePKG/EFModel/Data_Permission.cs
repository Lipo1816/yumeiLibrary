using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.EFModel
{
    public class Data_Permission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string 群組 { get; set; } = "";

        public string 工單看板 { get; set; } = "";
        public string 設備看板 { get; set; } = "";
        public string 品管看板 { get; set; } = "";
        public string 設備管理 { get; set; } = "";
        public string 設備點檢 { get; set; } = "";
        public string 工單報工 { get; set; } = "";
        public string 人員 { get; set; } = "";
        public string 資料分析 { get; set; } = "";
        public string 資料設定 { get; set; } = "";
    }
}
