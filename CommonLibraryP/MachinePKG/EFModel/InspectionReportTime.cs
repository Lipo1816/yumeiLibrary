using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.EFModel
{
   public class InspectionReportTime
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Type { get; set; } = ""; // "Daily", "Weekly", "Monthly"

        // 日檢用
        public TimeSpan? DailyTime { get; set; }
        public bool? DailyEnable { get; set; }

        // 周檢用
        public string? WeeklyDay { get; set; } // "星期一" ~ "星期日"
        public TimeSpan? WeeklyTime { get; set; }
        public bool? WeeklyEnable { get; set; }

        // 月檢用
        public int? MonthlyDay { get; set; } // 1~30
        public TimeSpan? MonthlyTime { get; set; }
        public bool? MonthlyEnable { get; set; }

        // 季檢用
        public int? QuarterMonth { get; set; } // 1~3
        public int? QuarterDay { get; set; }   // 1~30
        public int? QuarterHour { get; set; }  // 1~24
        public bool? QuarterEnable { get; set; }
        public TimeSpan? QuarterTime { get; set; } // 新增：季檢時間

        // 年檢用
        public int? YearMonth { get; set; }    // 1~12
        public int? YearDay { get; set; }      // 1~30
        public int? YearHour { get; set; }     // 1~24
        public bool? YearEnable { get; set; }
        public TimeSpan? YearTime { get; set; } // 新增：年檢時間
    }
}
