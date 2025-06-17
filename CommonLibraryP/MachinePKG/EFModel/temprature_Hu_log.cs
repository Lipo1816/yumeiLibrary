using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.EFModel
{
    public class temprature_Hu_log
    {
        [Key]
        public int Id { get; set; } // 主鍵，可自動遞增

        public string MachineName { get; set; } = string.Empty;

        [Required]
        public string MachineNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string MachineGroupNumber { get; set; } = string.Empty;

        public double? temperature { get; set; }
        public double? humidity { get; set; }
        public double? battery { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }
    }
}
