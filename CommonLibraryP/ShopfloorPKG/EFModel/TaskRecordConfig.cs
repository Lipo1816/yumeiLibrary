using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.ShopfloorPKG
{
    public partial class TaskRecordConfig
    {
        public Guid Id { get; set; }

        public string? TaskRecordsCategory { get; set; }

        public virtual ICollection<TaskRecordContent> TaskRecordContents { get; set; } = new List<TaskRecordContent>();

        public virtual ICollection<Workorder> Workorders { get; set; } = new List<Workorder>();
    }
}
