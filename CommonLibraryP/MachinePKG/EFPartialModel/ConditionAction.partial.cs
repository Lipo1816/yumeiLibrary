using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public abstract partial class ConditionAction
    {
        public abstract Task RunCommand(MachineService machineService);
    }
}
