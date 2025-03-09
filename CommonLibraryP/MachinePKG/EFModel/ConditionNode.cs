using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public abstract partial class ConditionNode
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        public Guid? ConditionId { get; set; }
        public virtual Condition? Condition { get; set; }
        public virtual Guid? ParentNodeId { get; set; }
        public virtual ConditionNode? ParentNode { get; set; }
        public virtual ICollection<ConditionNode> ChildNodes { get; set; } = new List<ConditionNode>();

        public abstract bool ContentValid { get; }

        public abstract string NameAndContent { get; }
    }
}
