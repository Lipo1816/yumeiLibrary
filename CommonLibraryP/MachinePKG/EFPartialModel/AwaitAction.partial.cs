using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public partial class AwaitAction : ConditionAction
    {
        public AwaitAction()
        {
        }
        public AwaitAction(Guid conditionId)
        {
            ConditionId = conditionId;
        }
    }
}
