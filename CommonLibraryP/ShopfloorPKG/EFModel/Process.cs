using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace CommonLibraryP.ShopfloorPKG
{
    public partial class Process
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        //public virtual ICollection<Machine> Machines { get; set; } = new List<Machine>();

        public virtual ICollection<Station> Stations { get; set; } = new List<Station>();

        public virtual ICollection<Workorder> Workorders { get; set; } = new List<Workorder>();
    }
}
