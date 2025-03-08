using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public partial class ModbusSlaveConfig
    {
        public Guid Id { get; set; }
        [Required]
        public string Ip { get; set; } = null!;
        [Required]
        public int Port { get; set; }
        [Required]
        public int Station { get; set; }
    }
}
