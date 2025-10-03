using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public partial class AwaitAction: ConditionAction  //lipo
    {
        [Range(1000, Int16.MaxValue)]
        public int DelayMillisecond { get; set; }

        public override int CommandCode { get; set; } = 0;

        public override async Task RunCommand(MachineService machineService)
        {
            await Task.Delay(DelayMillisecond);
        }
    }
}
