using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommonLibraryP.MachinePKG.EFModel
{
    public class InspectionList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string 機台編號 { get; set; } = "";

        [Required]
        public string 機台名稱 { get; set; } = "";

        [Required]
        public DateTime 產生時間 { get; set; }

        public DateTime? 完成時間 { get; set; }


        public string 檢查人 { get; set; } 

        [Required]
        public string TYPE { get; set; } = "";

        [Required]
        public InspectionFormStatus 表單狀態 { get; set; }

        [Required]
        public string 單號 { get; set; } = "";
    }
}