using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.EFModel
{
  public  class CarbonGeneratorParameter
    {
        [Key]
        public int Id { get; set; }

        [Required]


        ///PLC Name
        public string GeneratorName { get; set; } = "";

        public string? Model { get; set; } = "";

        public double? RatedPower { get; set; }

        public double? CarbonFactor { get; set; }

        // 新增欄位
        public double? Current { get; set; }         // 電流 (A)
        public double? Voltage { get; set; }         // 電壓 (V)
        public double? Electricity { get; set; }     // 用電量 (kWh)
        public DateTime? RecordTime { get; set; }    // 紀錄時間

        public string? Remark { get; set; }
    }
}
