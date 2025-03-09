using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public partial class SetTagAction
    {
        public SetTagAction()
        {
        }
        public SetTagAction(Guid conditionId)
        {
            ConditionId = conditionId;
        }
    }
}
