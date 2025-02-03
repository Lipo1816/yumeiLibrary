using CommonLibraryP.API;
using CommonLibraryP.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommonLibraryP.ShopfloorPKG
{
    
    public partial class Station
    {
        public Station() { }

        public Station(Guid processId)
        {
            ProcessId = processId;
            //Id = Guid.NewGuid();
            //Enable = true;
        }

        public bool IsSingleWorkorder => ((StationType / 100) % 10) switch
        {
            1 => true,
            2 => false,
            _ => throw new IndexOutOfRangeException($"IsSingleWorkorder flag error(value{(StationType / 100) % 10})"),
        };

        public bool IsSingleItem => ((StationType / 10) % 10) switch
        {
            1 => true,
            2 => false,
            _ => throw new IndexOutOfRangeException($"IsSingleItem flag error(value{(StationType / 10) % 10})"),
        };

        public bool WithSerialNo => (StationType % 10) switch
        {
            1 => true,
            2 => false,
            _ => throw new IndexOutOfRangeException($"WithSerialNo flag error(value{StationType % 10})"),
        };


        private Status stationStatus = Status.Init;
        public Status StationStatus => stationStatus;

        protected void SetStationStatus(Status s, string msg = "Normal")
        {
            stationStatus = s;
            SetErrorMsg(msg);
            UIUpdate();
        }

        private string errorMsg = String.Empty;
        public string ErrorMsg => errorMsg;

        private void SetErrorMsg(string s)
        {
            errorMsg = StationStatus is Status.Error ? s : "Normal";
        }
        protected void UIUpdate()
        {
            UIUpdateAct?.Invoke();
        }
        public Action? UIUpdateAct;


        public void InitStation()
        {
            stationStatus = Status.Init;
            errorMsg = String.Empty;
            UIUpdate();
        }

        protected List<Workorder> workorders = new();
        [NotMapped]
        public List<Workorder> Workorders => workorders;
        public virtual int WorkorderAmount => throw new NotImplementedException();
        public virtual bool WorkorderAmountValid => throw new NotImplementedException();
        public virtual bool Canrun => throw new NotImplementedException();
        protected List<ItemDetail> wipItemDetails = new();
        [NotMapped]
        public List<ItemDetail> WIPItemDetails => wipItemDetails;
        protected int ItemAmount => wipItemDetails.Count;
        public virtual bool ItemAmountValid => throw new NotImplementedException();
        public int TaskAmount => wipItemDetails.SelectMany(x=>x.TaskDetails).Count();
        public bool TaskAmountValid => wipItemDetails.TrueForAll(x => x.OneTaskValid);

        public virtual RequestResult SetWorkorder(Workorder wo)
        {
            return new(4, "not implement yet");
        }

        public virtual RequestResult ClearWorkorder()
        {
            return new(4, "not implement yet");
        }

        public virtual RequestResult Run()
        {
            return new(4, "not implement yet");
        }

        public virtual RequestResult CheckCanAddItem()
        {
            return new(4, "not implement yet");
        }

        public virtual RequestResult AddItemDetail(ItemDetail itemDetail)
        {
            return new(4, "not implement yet");
        }

        public virtual RequestResult CheckCanRemoveItem()
        {
            return new(4, "not implement yet");
        }

        public virtual RequestResult RemoveItemDetail()
        {
            return new(4, "not implement yet");
        }

        public virtual RequestResult Pause()
        {
            return new(4, "not implement yet");
        }

        public virtual RequestResult Stop()
        {
            return new(4, "not implement yet");
        }

        protected void Error(string s)
        {
            stationStatus = Status.Error;
            SetErrorMsg(s);
            UIUpdate();
        }
    }
}
