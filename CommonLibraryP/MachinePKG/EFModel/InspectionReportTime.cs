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

        // 周檢用
        public string? WeeklyDay { get; set; } // "星期一" ~ "星期日"
        public TimeSpan? WeeklyTime { get; set; }

        // 月檢用
        public int? MonthlyDay { get; set; } // 1~30
        public TimeSpan? MonthlyTime { get; set; }
    }
}
