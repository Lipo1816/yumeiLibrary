using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public class SetMachineTagModel
    {
        [Required]
        public string MachineName { get; set; } = null!;
        [Required]
        public string TagName { get; set; } = null!;
        public string ValueString { get; set; } = null!;
    }
}
