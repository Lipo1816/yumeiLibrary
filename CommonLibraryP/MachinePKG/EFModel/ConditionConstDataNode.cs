using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public partial class ConditionConstDataNode : ConditionNode
    {
        [Required]
        public override Guid? ParentNodeId { get => base.ParentNodeId; set => base.ParentNodeId = value; }
        [Required]
        public string Value { get; set; } = null!;

        public override bool ContentValid => !string.IsNullOrEmpty(Value);

        public override string NameAndContent => $"{Name}({Value})";

        public override object GetNodeValue(MachineService machineService)
        {
            return Value;
        }
    }
}
