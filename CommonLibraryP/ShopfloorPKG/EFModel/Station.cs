using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.ShopfloorPKG
{
    [Index(nameof(Name))]
    public partial class Station
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid ProcessId { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public int ProcessIndex { get; set; }
        [Required]
        public int StationType { get; set; }
        [Required]
        [DefaultValue(true)]
        public bool Enable { get; set; }

        public virtual Process Process { get; set; } = null!;

        public virtual ICollection<TaskDetail> TaskDetails { get; set; } = new List<TaskDetail>();
    }
}
