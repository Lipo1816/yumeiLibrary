using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.ShopfloorPKG
{
    public partial class ItemRecordDetail
    {
        public Guid ItemId { get; set; }

        public Guid RecordContentId { get; set; }

        public string? Value { get; set; }
        public virtual ItemDetail Item { get; set; } = null!;
        public virtual ItemRecordContent RecordContent { get; set; } = null!;
    }
}
