using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public partial class ConditionConstDataNode
    {
        public ConditionConstDataNode() : base()
        {

        }
        public ConditionConstDataNode(Guid conditionId)
        {
            ConditionId = conditionId;
        }
    }
}
