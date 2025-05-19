using CommonLibraryP.API;
using CommonLibraryP.Data;
using CommonLibraryP.MachinePKG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.ShopfloorPKG
{
    public class StationSingleWorkorderSingleSerial : StationSingleWorkorder
    {


        public StationSingleWorkorderSingleSerial(Station station) : base(station)
        {

        }

        public override bool ItemAmountValid => wipItemDetails.Count >= 0 && wipItemDetails.Count <= 1;

        public override RequestResult CheckCanAddItem()
        {
            if (StationStatusCode is not 5)
            {
                return new RequestResult(4, $"Station {Name} is not running");
            }
            if (WorkorderAmount is not 1)
            {
                return new RequestResult(4, $"Station {Name} workorder amount error ({WorkorderAmount})");
            }
            if (ItemAmount is not 0)
            {
                return new RequestResult(4, $"Station {Name} item amount error ({ItemAmount})");
            }
            if (!TaskAmountValid)
            {
                return new RequestResult(4, $"Station {Name} task amount error ({TaskAmount})");
            }
            return new RequestResult(2, $"Station {Name} can add item");
        }
        public override RequestResult AddItemDetail(ItemDetail itemDetail)
        {
            var check = CheckCanAddItem();
            if (!check.IsSuccess)
            {
                return check;
            }
            if (itemDetail.TaskDetails.Count is not 1)
            {
                return new RequestResult(4, $"Item {itemDetail.SerialNo} task amount {itemDetail.TaskDetails.Count} error");
            }
            wipItemDetails.Add(itemDetail);
            UIUpdate();
            return new RequestResult(2, $"Station {Name} add item {itemDetail?.SerialNo} success");
        }
        public override RequestResult CheckCanRemoveItem()
        {
            if (StationStatusCode is not 5)
            {
                return new RequestResult(4, $"Station {Name} is not running");
            }
            if (WorkorderAmount is not 1)
            {
                return new RequestResult(4, $"Station {Name} has no workorder yet");
            }
            if (ItemAmount is not 1)
            {
                return new RequestResult(4, $"Station {Name} has no item yet");
            }
            if (!TaskAmountValid)
            {
                return new RequestResult(4, $"Station {Name} task amount error {TaskAmount}");
            }
            return new RequestResult(2, $"Station {Name} can remove item");
        }
        public override RequestResult RemoveItemDetail()
        {
            var check = CheckCanRemoveItem();
            if (!check.IsSuccess)
            {
                return check;
            }
            if (!TaskAmountValid)
            {
                return new RequestResult(4, $"Item task amount {TaskAmount} error");;
            }
            wipItemDetails.Clear();
            UIUpdate();
            return new RequestResult(2, $"Station {Name} remove item success");
        }
    }
}
