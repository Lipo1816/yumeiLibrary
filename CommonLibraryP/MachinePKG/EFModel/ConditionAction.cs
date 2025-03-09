using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public abstract partial class ConditionAction
    {
        public Guid? Id { get; set; }
        public Guid? ConditionId { get; set; }

        public virtual Condition? Condition { get; set; }

        public string Name { get; set; } = null!;
        [NotMapped]
        public abstract int CommandCode { get; set; }

        [Range(0, 100)]
        public int Sequence { get; set; }
    }
}
