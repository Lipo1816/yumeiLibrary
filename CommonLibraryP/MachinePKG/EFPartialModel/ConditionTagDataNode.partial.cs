using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public partial class ConditionTagDataNode
    {
        public override bool ContentValid => TargetMachineId != Guid.Empty && TargetTagId != Guid.Empty;

        public override string NameAndContent => $"{Name}(?)";


        public ConditionTagDataNode() : base()
        {

        }
        public ConditionTagDataNode(Guid conditionId)
        {
            ConditionId = conditionId;
        }

        public override object GetNodeValue(MachineService machineService)
        {
            var tag = machineService.GetMachineTagById(TargetMachineId, TargetTagId);
            return tag is null? string.Empty : tag.ValueString;
        }
    }
}
