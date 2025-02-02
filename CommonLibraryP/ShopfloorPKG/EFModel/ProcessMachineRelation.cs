using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.ShopfloorPKG
{
    public partial class ProcessMachineRelation
    {
        public Guid? Id { get; set; }
        [Required]
        public Guid? ProcessId { get; set; }
        [Required]
        public Guid? MachineId { get; set; }
    }
}
