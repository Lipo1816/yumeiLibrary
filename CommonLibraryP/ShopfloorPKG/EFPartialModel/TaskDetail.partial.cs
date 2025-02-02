using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.ShopfloorPKG
{
    public partial class TaskDetail
    {
        public TaskDetail()
        {
        }

        public TaskDetail(Guid itemID, Guid stationID)
        {
            Id = Guid.NewGuid();
            ItemId = itemID;
            StationId = stationID;
            StartTime = DateTime.Now;
        }
    }
}
