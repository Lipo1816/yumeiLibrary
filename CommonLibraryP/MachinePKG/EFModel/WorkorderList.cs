using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommonLibraryP.MachinePKG.EFModel
{
    public class WorkorderList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string 工單號 { get; set; } = "";

        [Required]
        public DateTime 報工時間 { get; set; }

        [Required]
        public string 狀態 { get; set; } = "";

        [Required]
        public string 報工人員 { get; set; } = "";
    }
}