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
        public string 狀態 { get; set; } = "";

        public DateTime 報工時間 { get; set; }

        public DateTime 產生時間 { get; set; }

        public string 報工人員 { get; set; } = "";
        [Required]
        public string? Type { get; set; }

        // 新增欄位
        public string? 錯誤項目1 { get; set; }
        public string? 錯誤項目2 { get; set; }
        public string? 錯誤項目3 { get; set; }
        public string? 錯誤項目4 { get; set; }
        public string? 錯誤描述 { get; set; }

        public bool? result { get; set; }
    }
}
