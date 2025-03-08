using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public partial class ConditionNode
    {
        public bool IsLeaf => ChildrenNodes.Count() == 0 && LogicalOperation < 0;
        public bool IsRoot => ParentNode is null && Condition is not null;
        public bool BindingToTag => MachineId is not null && TagId is not null;

        public string Symbol => MachineTypeEnumHelper.GetLogicalOperationSymbol(LogicalOperation);
    }
}
