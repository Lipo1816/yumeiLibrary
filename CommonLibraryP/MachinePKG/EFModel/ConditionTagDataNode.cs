using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public partial class ConditionTagDataNode : ConditionNode
    {
        [Required]
        public override Guid? ParentNodeId { get => base.ParentNodeId; set => base.ParentNodeId = value; }
        [Required]
        public Guid TargetMachineId { get; set; }
        [Required]
        public Guid TargetTagId { get; set; }

    }
}
