using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.EFModel
{
    public class TagRecordData
    {
        [Key]
        [Required]
        [StringLength(100)]
        public string tagName { get; set; } = string.Empty;

        [Required]
        public Guid tagID { get; set; }

        [Required]
        public Guid CategoryID { get; set; }

        [Required]
        public DateTime saveDate { get; set; }
    }
}
