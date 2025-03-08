using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public partial class ConditionNode
    {
        public Guid Id { get; set; }
        public Guid? ConditionId { get; set; }
        public virtual Condition? Condition { get; set; }
        public Guid? ParentNodeId { get; set; }
        public virtual ConditionNode? ParentNode { get; set; }
        public int LeafPosition { get; set; }
        public virtual ICollection<ConditionNode> ChildrenNodes { get; set; } = new List<ConditionNode>();
        public Guid? MachineId { get; set; }
        public Guid? TagId { get; set; }
        public int LogicalOperation
        {
            get
            {
                return logicalOperation;
            }
            set
            {
                logicalOperation = value;
                if (logicalOperation > 0)
                {
                    MachineId = null;
                    TagId = null;
                }
            }
        }

        private int logicalOperation;
    }
}
