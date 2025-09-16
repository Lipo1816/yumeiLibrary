using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.EFModel
{
    public class EmailSentSetting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string 人員ID { get; set; } = "";

        public bool ischoose { get; set; }

        public DateTime? 建立時間 { get; set; }

    }
}
