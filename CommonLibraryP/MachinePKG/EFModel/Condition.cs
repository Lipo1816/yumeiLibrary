using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public partial class Condition
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public bool Enable { get; set; }
        public virtual ICollection<ConditionNode> ConditionNodes { get; set; } = new List<ConditionNode>();

        public virtual ICollection<ConditionAction> ConditionActions { get; set; } = new List<ConditionAction>();
    }
}
