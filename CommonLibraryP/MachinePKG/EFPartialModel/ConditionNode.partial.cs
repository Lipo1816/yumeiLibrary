using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public abstract partial class ConditionNode
    {
        public bool AllValid => ContentValid && ChildNodes.All(x => x.ContentValid);

        public abstract Object GetNodeValue(MachineService machineService);
    }
}
