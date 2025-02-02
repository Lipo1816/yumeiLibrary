using CommonLibraryP.API;
using CommonLibraryP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.ShopfloorPKG
{
    public abstract class StationSingleWorkorder : Station
    {

        public StationSingleWorkorder(Station station)
        {
            Id = station.Id;
            ProcessId = station.ProcessId;
            Name = station.Name;
            ProcessIndex = station.ProcessIndex;
            StationType = station.StationType;
            Process = station.Process;
            Enable = station.Enable;
        }

        //private Workorder? workorder;
        public override int WorkorderAmount => workorders.Count;

        public override bool WorkorderAmountValid => WorkorderAmount >= 0 && WorkorderAmount <= 1;

        public override bool Canrun => StationStatus is Status.Init && WorkorderAmountValid && WorkorderAmount is 1;

        public override RequestResult SetWorkorder(Workorder wo)
        {
            if (WorkorderAmountValid && WorkorderAmount is 1)
            {
                return new(4, "Workorder already exist");
            }
            if (StationStatus is not Status.Init)
            {
                return new(4, "Station is not at Init status");
            }
            workorders.Add(wo);
            UIUpdate();
            return new(2, $"Station {Name} set workorder {wo.WorkorderNo}-{wo.Lot} success");
        }

        public override RequestResult ClearWorkorder()
        {
            if (WorkorderAmountValid && WorkorderAmount is 0)
            {
                return new(4, $"Station {Name} has no workorder");
            }
            else
            {
                workorders.Clear();
                UIUpdate();
                return new(2, $"Station {Name} clear workorder success");
            }
        }
    }
}
