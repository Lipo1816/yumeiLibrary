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


        private int stationStatusCode = 0;
        public int StationStatusCode => stationStatusCode;

        protected void SetStationStatus(int statusCode, string msg = "Normal")
        {
            stationStatusCode = statusCode;
            SetErrorMsg(msg);
            UIUpdate();
        }

        private string errorMsg = String.Empty;
        public string ErrorMsg => errorMsg;

        private void SetErrorMsg(string s)
        {
            errorMsg = stationStatusCode is 8 ? s : "Normal";
        }
        protected void UIUpdate()
        {
            UIUpdateAct?.Invoke();
        }
        public Func<Task>? UIUpdateAct;


        public void InitStation()
        {
            stationStatusCode = 0;
            errorMsg = String.Empty;
            UIUpdate();
        }

        #region workorder
        protected List<Workorder> workorders = new();
        [NotMapped]
        public List<Workorder> Workorders => workorders;
        public virtual int WorkorderAmount => throw new NotImplementedException();
        public virtual bool WorkorderAmountValid => throw new NotImplementedException();
        public virtual bool CanDeployWorkorder => throw new NotImplementedException();
        public virtual bool Canrun => throw new NotImplementedException();
        #endregion

        #region workorder operation
        public virtual RequestResult SetWorkorder(Workorder wo)
        {
            return new(4, "not implement yet");
        }
        public virtual RequestResult ClearWorkorder()
        {
            return new(4, "not implement yet");
        }

        #endregion

        #region item
        protected List<ItemDetail> wipItemDetails = new();
        [NotMapped]
        public List<ItemDetail> WIPItemDetails => wipItemDetails;
        protected int ItemAmount => wipItemDetails.Count;
        public virtual bool ItemAmountValid => throw new NotImplementedException();
        #endregion

        #region item operation
        public virtual RequestResult CheckCanAddItem()
        {
            return new(4, "not implement yet");
        }
        public bool CanStationIn => CheckCanAddItem().IsSuccess;

        public virtual RequestResult AddItemDetail(ItemDetail itemDetail)
        {
            return new(4, "not implement yet");
        }

        public virtual RequestResult CheckCanRemoveItem()
        {
            return new(4, "not implement yet");
        }
        public bool CanStationOut => CheckCanRemoveItem().IsSuccess;
        public virtual RequestResult RemoveItemDetail()
        {
            return new(4, "not implement yet");
        }
        #endregion

        #region task
        public int TaskAmount => wipItemDetails.SelectMany(x => x.TaskDetails).Count();
        public bool TaskAmountValid => wipItemDetails.TrueForAll(x => x.OneTaskValid);
        #endregion

        #region station status
        public virtual RequestResult Run()
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
            stationStatusCode = 8;
            SetErrorMsg(s);
            UIUpdate();
        }
        #endregion
    }
}
