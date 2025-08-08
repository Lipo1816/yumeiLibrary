using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.EFModel
{
   public class Inspection_WoItem
    {
        public int Index { get; set; } // ← 新增這行
        [Key, Column(Order = 0)]
        [Required]
        public string 點檢單號 { get; set; } = string.Empty;

        [Key, Column(Order = 1)]
        [Required]
        public string 點檢項目 { get; set; } = string.Empty;
        public string? 錯誤代碼 { get; set; }   // 新增
        public string? 分類 { get; set; }       // 新增
        [Required]
        public DateTime 點檢時間 { get; set; }

        public string? 點檢內容 { get; set; }

        public string? 錯誤項目 { get; set; }

        public string? 備註 { get; set; }

        public string? 責任單位 { get; set; }

        public bool? 結果 { get; set; }
    }
}
