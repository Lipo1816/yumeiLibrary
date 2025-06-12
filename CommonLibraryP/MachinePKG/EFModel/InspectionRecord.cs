using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.EFModel
{
    public enum InspectionFormStatus
    {
        UndoCheck,
        CheckDone,
        ModifyCheck,
        NoUse
    }

    public class InspectionRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string 機台編號 { get; set; } = "";

        [Required]
        public string 機台名稱 { get; set; } = "";

        [Required]
        public string 項目 { get; set; } = "";

        public string? 紀錄值 { get; set; } // 可為 null

        [Required]
        public DateTime 產生時間 { get; set; }

        public DateTime? 完成時間 { get; set; } // 可為 null

        [Required]
        public string 檢查人 { get; set; } = "";

        [Required]
        public InspectionFormStatus 表單狀態 { get; set; }

        [Required]
        public string 檢查單號 { get; set; } = "";
        

    }
}
