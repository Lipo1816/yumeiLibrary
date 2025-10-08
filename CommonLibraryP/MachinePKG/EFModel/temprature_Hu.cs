using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.EFModel
{
    public class temprature_Hu
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string MachineName { get; set; } = string.Empty;

        [Required]
        public string MachineNumber { get; set; } = string.Empty;

        [Required]
        public string MachineGroupNumber { get; set; } = string.Empty;

        public double? temperature_high { get; set; }
        public double? temperature_low { get; set; }
        public double? humidity_high { get; set; }
        public double? humidity_low { get; set; }
        public double? battery_high { get; set; }
        public double? battery_low { get; set; }
        // 新增 ChangeType 屬性
        //public string? ChangeType { get; set; }
    }
}
