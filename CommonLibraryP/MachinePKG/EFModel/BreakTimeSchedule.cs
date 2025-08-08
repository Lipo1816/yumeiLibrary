using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.EFModel
{
   public class BreakTimeSchedule
    {
        public string LineName { get; set; } = string.Empty;
        public string WeekDay { get; set; } = string.Empty;
        public int PeriodNo { get; set; }
        public DateTime? StartTime { get; set; }   // 改成 DateTime?
        public DateTime? EndTime { get; set; }     // 改成 DateTime?
        public DateTime ModifyTime { get; set; }
        public bool IsEnable { get; set; }
    }
}
