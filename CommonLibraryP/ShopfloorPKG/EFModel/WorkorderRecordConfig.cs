using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.ShopfloorPKG
{
    public partial class WorkorderRecordConfig
    {
        public Guid Id { get; set; }
        [Required]
        public string? WorkorderRecordCategory { get; set; }

        public virtual ICollection<WorkorderRecordContent> WorkorderRecordContents { get; set; } = new List<WorkorderRecordContent>();

        public virtual ICollection<Workorder> Workorders { get; set; } = new List<Workorder>();
    }
}
