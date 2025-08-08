using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommonLibraryP.MachinePKG.EFModel
{
    public class InspecWOList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; } // 自動增加號碼

        [Required]
        public string 工單 { get; set; } = "";

        [Required]
        public string 點檢單號 { get; set; } = "";

        [Required]
        public InspectionFormStatus 狀態 { get; set; } = InspectionFormStatus.UndoCheck;

        public DateTime 報工時間 { get; set; }

        public DateTime 產生時間 { get; set; }

        public string 報工人員 { get; set; } = "";
        [Required]
        public string? Type { get; set; }

        // 新增改善時間
        public DateTime? 改善時間 { get; set; }

        public bool? result { get; set; }
        public int 點檢時間 { get; set; }

        public string? 錯誤代碼 { get; set; }   // 新增
        public string? 分類 { get; set; }       // 新增


        public string? 錯誤項目 { get; set; }

        public string? 備註 { get; set; }

        public string? 責任單位 { get; set; }
        public int? 檢查數量 { get; set; }
        public int? NG數量 { get; set; }
    }
}
