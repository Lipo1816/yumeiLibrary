using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public partial class SetTagAction : ConditionAction
    {
        public Guid? TargetMachineId { get; set; }
        public Guid? TargetTagId { get; set; }
        public string ValueString { get; set; } = null!;

        public override int CommandCode { get; set; } = 1;

        public override async Task RunCommand(MachineService machineService)
        {
            await machineService.SetMachineTagByIdAndString(TargetMachineId, TargetTagId, ValueString);
        }
    }
}
