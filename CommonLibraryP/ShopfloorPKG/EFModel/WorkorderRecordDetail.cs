using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.ShopfloorPKG
{
    public partial class WorkorderRecordDetail
    {
        public Guid WorkerderId { get; set; }

        public Guid RecordContentId { get; set; }

        public string? Value { get; set; }

        public virtual WorkorderRecordContent RecordContent { get; set; } = null!;

        public virtual Workorder Workerder { get; set; } = null!;
    }
}
