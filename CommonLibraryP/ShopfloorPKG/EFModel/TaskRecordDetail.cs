using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.ShopfloorPKG
{
    public partial class TaskRecordDetail
    {
        public Guid TaskId { get; set; }

        public Guid RecordContentId { get; set; }

        public string? Value { get; set; }

        public virtual TaskRecordContent RecordContent { get; set; } = null!;

        public virtual TaskDetail Task { get; set; } = null!;
    }
}
