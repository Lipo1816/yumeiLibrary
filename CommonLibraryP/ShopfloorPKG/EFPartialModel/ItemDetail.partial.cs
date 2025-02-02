using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.ShopfloorPKG
{
    public partial class ItemDetail
    {
        public ItemDetail()
        {
        }

        public ItemDetail(Guid workorderID, string serialNo)
        {
            Id = Guid.NewGuid();
            WorkordersId = workorderID;
            SerialNo = serialNo;
            TargetAmount = 1;
            Okamount = 0;
            Ngamount = 0;
            StartTime = DateTime.Now;
        }

        public bool OneTaskValid => TaskDetails.Count == 1;
    }
}
