using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public partial class ConditionLogicNode: ConditionNode
    {
        [Range(1, 6)]
        public int LogicalOperation { get; set; }


        
    }
}
