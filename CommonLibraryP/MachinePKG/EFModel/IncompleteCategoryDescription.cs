using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommonLibraryP.MachinePKG.EFModel
{
    public class IncompleteCategoryDescription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string 未完成類別 { get; set; } = "";

        [Required]
        public string 未完成說明 { get; set; } = "";

        public int 排序順序 { get; set; } = 0;
    }
}
