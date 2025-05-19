﻿using CommonLibraryP.API;
using CommonLibraryP.Data;
using CommonLibraryP.MachinePKG;
using DevExpress.Data.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using static System.Collections.Specialized.BitVector32;

namespace CommonLibraryP.ShopfloorPKG
{
    public class ShopfloorService
    {
        private readonly IServiceScopeFactory scopeFactory;
        public ShopfloorService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        #region process
        public async Task<List<Process>> GetAllProcesses()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShopfloorDBContext>();
                return await dbContext.Processes.AsNoTracking().ToListAsync();
            }
        }
        public async Task<RequestResult> UpsertProcess(Process process)
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ShopfloorDBContext>();
                    Process? targetProcess = dbContext.Processes.Include(x => x.Stations).FirstOrDefault(x => x.Id == process.Id);
                    if (targetProcess != null)
                    {
                        targetProcess.Name = process.Name;
                    }
                    else
                    {
                        await dbContext.Processes.AddAsync(process);
                    }
                    await dbContext.SaveChangesAsync();
                    return new RequestResult(2, $"Upsert process {process.Name} success");
                }
            }
            catch (Exception ex)
            {
                return new RequestResult(4, $"Upsert process {process.Name} fail({ex.Message})");
            }
        }
        public async Task<RequestResult> DeleteProcess(Process process)
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ShopfloorDBContext>();
                    Process? targetProcess = dbContext.Processes.Include(x => x.Stations).FirstOrDefault(x => x.Id == process.Id);
                    if (targetProcess != null)
                    {
                        dbContext.Remove(targetProcess);
                        await dbContext.SaveChangesAsync();
                        return new RequestResult(2, $"Delete process {targetProcess.Name} success");
                    }
                    else
                    {
                        return new RequestResult(4, $"Process {process.Name} not found");
                    }

                }
            }
            catch (Exception ex)
            {
                return new RequestResult(4, $"Delete process {process.Name} fail({ex.Message})");
            }
        }
        public async Task<List<Process>> GetAllProcessAndStations()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShopfloorDBContext>();
                return await dbContext.Processes.Include(x => x.Stations.OrderBy(x => x.ProcessIndex).ThenBy(x => x.Name)).AsNoTracking().ToListAsync();
            }
        }
        public Task<List<ProcessMachineRelation>> GetProcessMachineRelationByID(Guid? id)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShopfloorDBContext>();
                var targetMachinesId = dbContext.ProcessMachineRelations.Where(x => x.ProcessId == id);
                return Task.FromResult(targetMachinesId.ToList());
            }
        }
        public async Task<Process?> GetProcessByName(string processName)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShopfloorDBContext>();
                return await dbContext.Processes.FirstOrDefaultAsync(x => x.Name == processName);
            }
        }
        public async Task<Process?> GetProcessByID(Guid? processID)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShopfloorDBContext>();
                return await dbContext.Processes.FirstOrDefaultAsync(x => x.Id == processID);
            }
        }

        public async Task<List<Station>> GetStationsByProcessID(Guid processID)
        {
            Process? targetProcess = await GetProcessByID(processID);
            if (targetProcess is not null)
            {
                return Stations.Where(x => x.ProcessId == targetProcess.Id).ToList();
            }
            return new();
        }

        private async Task<bool> CheckStationIsLastInProcess(Station station)
        {
            var stations = await GetStationsByProcessID(station.ProcessId);
            return stations.Where(x => x.Enable == true).Max(x => x.ProcessIndex) == station.ProcessIndex;
        }
        #endregion

        #region station

        private List<Station> stations = new List<Station>();
        public List<Station> Stations => stations;

        public async Task<List<Station>> GetAllStationConfigs()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShopfloorDBContext>();
                return await dbContext.Stations.AsNoTracking().ToListAsync();
            }
        }

        public virtual List<StationTypeWrapperClass> GetStationTypesWrapperClass()
        {
            return ShopfloorTypeEnumHelper.GetStationTypesWrapperClass().ToList();
        }
        public async Task<IEnumerable<Machine>> GetMachineConfigs()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var machineService = scope.ServiceProvider.GetRequiredService<MachineService>();
                    return await machineService.GetAllMachinesConfig();
                }
                catch
                {
                    return new List<Machine>();
                }

            }
        }
        public async Task<RequestResult> UpsertStation(Station station)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ShopfloorDBContext>();
                    Station? target = dbContext.Stations.FirstOrDefault(x => x.Id == station.Id);
                    if (target != null)
                    {
                        target.ProcessId = station.ProcessId;
                        target.Name = station.Name;
                        target.ProcessIndex = station.ProcessIndex;
                        target.StationType = station.StationType;
                        target.Enable = station.Enable;

                    }
                    else
                    {
                        await dbContext.Stations.AddAsync(station);
                    }
                    await dbContext.SaveChangesAsync();
                    return new(2, $"Upsert station {station.Name} success");
                }
                catch (Exception ex)
                {
                    return new RequestResult(4, $"Upsert station {station.Name} fail({ex.Message})");
                }

            }
        }
        public async Task<RequestResult> DeleteStation(Station station)
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ShopfloorDBContext>();
                    Station? targetStation = dbContext.Stations.FirstOrDefault(x => x.Id == station.Id);
                    if (targetStation != null)
                    {
                        dbContext.Remove(targetStation);
                        await dbContext.SaveChangesAsync();
                        return new RequestResult(2, $"Delete station {targetStation.Name} success");
                    }
                    else
                    {
                        return new RequestResult(4, $"Process {station.Name} not found");
                    }

                }
            }
            catch (Exception ex)
            {
                return new RequestResult(4, $"Delete station {station.Name} fail({ex.Message})");
            }
        }
        protected virtual Station InitMachineToDerivesClass(Station station)
        {
            Station res;
            switch (station.StationType)
            {
                case 111:
                    res = new StationSingleWorkorderSingleSerial(station);
                    break;
                default:
                    res = station;
                    break;
            }
            //res.MachineStatechangedRecordAct += MachineStatusChangedRecord;
            return res;
        }
        public async Task InitAllStation()
        {
            stations = new();
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShopfloorDBContext>();
                var stationbases = await dbContext.Stations.Include(x => x.Process).AsNoTracking().ToListAsync();
                foreach (var stationbase in stationbases)
                {
                    stations.Add(InitMachineToDerivesClass(stationbase));
                }
                stations.ForEach(x => x.InitStation());
            }
        }

        private Station? GetStationByName(string stationName)
        {
            return stations.FirstOrDefault(x => x.Name == stationName);
        }

        public async Task<RequestResult> DeployWorkorderToStation(Guid stationId, WorkorderIdModel WorkorderIdModel)
        {
            var workorder = await GetWorkorderById(WorkorderIdModel.WorkorderID);
            var station = stations.FirstOrDefault(x => x.Id == stationId);
            if (workorder is not null && station is not null)
            {
                station.SetWorkorder(workorder);
                return new(2, $"Deploy workorder {workorder.WorkorderNo}-{workorder.Lot} to station {station.Name} success");
            }
            return new(4, $"Workorder or station not found");
        }

        public Task<RequestResult> RunStation(Guid id)
        {
            var target = stations.FirstOrDefault(x => x.Id == id);
            if (target is not null)
            {
                return Task.FromResult<RequestResult>(target.Run());
            }
            return Task.FromResult<RequestResult>(new(3, "Station not found"));
        }

        public async Task<RequestResult> StationInByNameAndSerialNo(string stationName, string serialNo)
        {
            Station? targetStation = GetStationByName(stationName);
            if (targetStation is null)
            {
                return new(3, $"station {stationName} not found");
            }
            var checkStationInRes = targetStation.CheckCanAddItem();
            if (!checkStationInRes.IsSuccess)
            {
                return checkStationInRes;
            }
            switch (targetStation.StationType)
            {
                case 111:
                    try
                    {
                        StationSingleWorkorder? stationSingleWorkorder = targetStation as StationSingleWorkorder;
                        var itemDetail = await GetOrGenerateItemWithTaskDetail(stationSingleWorkorder.Workorders.FirstOrDefault().Id, serialNo, stationSingleWorkorder.Id);
                        var addRes = stationSingleWorkorder.AddItemDetail(itemDetail);
                        return addRes;
                    }
                    catch (Exception ex)
                    {
                        return new(4, ex.Message);
                    }
                default:
                    return new(3, $"station {stationName} deosn't support this command");
            }
        }

        public async Task<RequestResult> StationOutByFIFO(string stationName, bool pass)
        {
            Station? targetStation = GetStationByName(stationName);
            if (targetStation is null)
            {
                return new(3, $"station {stationName} not found");
            }
            var check = targetStation.CheckCanRemoveItem();
            if (!check.IsSuccess)
            {
                return check;
            }
            bool isLast = await CheckStationIsLastInProcess(targetStation);
            switch (targetStation.StationType)
            {
                case 111:
                    try
                    {
                        if (targetStation is StationSingleWorkorderSingleSerial stationSingleWorkorderSingleSerial)
                        {
                            var item = stationSingleWorkorderSingleSerial?.WIPItemDetails.FirstOrDefault();
                            stationSingleWorkorderSingleSerial?.RemoveItemDetail();

                            var taskDetail = item?.TaskDetails.FirstOrDefault();
                            taskDetail.FinishedTime = DateTime.Now;
                            await UpsertTaskDetail(taskDetail);
                            if (isLast)
                            {
                                item.FinishedTime = DateTime.Now;
                                if (pass)
                                {
                                    item.Okamount++;
                                }
                                else
                                {
                                    item.Ngamount++;
                                }
                                return await UpsertItemDetail(item);
                            }
                            return new(2, $"Station {stationName} station out by FIFO success");
                        }
                        else
                        {
                            return new(4, $"Station {stationName} type downcasting error");
                        }

                    }
                    catch (Exception ex)
                    {
                        return new(4, ex.Message);
                    }
                default:
                    return new(3, $"station {stationName} deosn't support this command");
            }
        }


        #endregion

        #region workorder
        public async Task<List<Workorder>> GetAllWorkordersConfig()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                Workorder w = new();
                var dbContext = scope.ServiceProvider.GetRequiredService<ShopfloorDBContext>();
                return await dbContext.Workorders.AsNoTracking().ToListAsync();
            }
        }
        public async Task<RequestResult> UpsertWorkorderConfig(Workorder workorder)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ShopfloorDBContext>();
                    var target = dbContext.Workorders.FirstOrDefault(x => x.Id == workorder.Id);
                    if (target != null)
                    {
                        target.ProcessId = workorder.ProcessId;
                        target.WorkorderNo = workorder.WorkorderNo;
                        target.Lot = workorder.Lot;
                        target.PartNo = workorder.PartNo;
                        target.RecipeCategoryId = workorder.RecipeCategoryId;
                        target.WorkorderRecordCategoryId = workorder.WorkorderRecordCategoryId;
                        target.ItemRecordsCategoryId = workorder.ItemRecordsCategoryId;
                        target.TaskRecordCategoryId = workorder.TaskRecordCategoryId;
                        target.TargetAmount = workorder.TargetAmount;
                    }
                    else
                    {
                        await dbContext.AddAsync(workorder);
                    }
                    await dbContext.SaveChangesAsync();
                    return new(2, $"Upsert workorder {workorder.WorkorderNo}/{workorder.Lot} success");
                }
                catch (Exception e)
                {
                    return new(4, $"Upsert workorder {workorder.WorkorderNo}/{workorder.Lot} fail({e.Message})");
                }

            }
        }
        public async Task<RequestResult> DeleteWorkorder(Workorder workorder)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ShopfloorDBContext>();
                    var target = dbContext.Workorders.FirstOrDefault(x => x.Id == workorder.Id);
                    if (target != null)
                    {
                        dbContext.Workorders.Remove(target);
                        await dbContext.SaveChangesAsync();
                        return new(2, $"Delete workorder {workorder.WorkorderNo}/{workorder.Lot} success");
                    }
                    else
                    {
                        return new(4, $"Workorder {workorder.WorkorderNo}/{workorder.Lot} not found");
                    }
                }
                catch (Exception e)
                {
                    return new(4, $"Delete workorder {workorder.WorkorderNo}/{workorder.Lot} fail({e.Message})");
                }


            }
        }
        public async Task<Workorder?> GetWorkorderById(Guid id)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShopfloorDBContext>();
                return await dbContext.Workorders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            }
        }
        public async Task<Workorder?> GetWorkorderByNoAndLot(string no, string lot)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShopfloorDBContext>();
                return await dbContext.Workorders.AsNoTracking().FirstOrDefaultAsync(x => x.WorkorderNo == no && x.Lot == lot);
            }
        }
        public async Task<List<Workorder>> GetRunningWorkordersByProcessID(Guid processId)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShopfloorDBContext>();
                return await dbContext.Workorders.Where(x => x.ProcessId == processId && x.Status == 5)
                    .AsNoTracking().ToListAsync();
            }
        }
        #endregion

        #region item

        private async Task<ItemDetail> GetOrGenerateItemWithTaskDetail(Guid workorderID, string serialNo, Guid stationID)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShopfloorDBContext>();
                var targetItem = dbContext.ItemDetails.Include(x => x.TaskDetails.Where(y => y.StationId == stationID))
                    .AsNoTracking()
                    .AsSplitQuery()
                    .FirstOrDefault(x => x.WorkordersId == workorderID && x.SerialNo == serialNo);
                if (targetItem is null)
                {
                    //first item
                    var newItem = await GenerateItemDetail(workorderID, serialNo);
                    var newTask = await GenerateTaskDetail(newItem.Id, stationID);
                    newItem.TaskDetails.Add(newTask);
                    return newItem;
                }
                else
                {
                    if (targetItem.TaskDetails.Count is 0)
                    {
                        //first task
                        var newTask = await GenerateTaskDetail(targetItem.Id, stationID);
                        targetItem.TaskDetails.Add(newTask);
                        return targetItem;
                    }
                    else if (targetItem.TaskDetails.Count is 1)
                    {
                        return targetItem;
                    }
                    else
                    {
                        throw new Exception($"Item {serialNo} task amount {targetItem.TaskDetails.Count} error");
                    }
                }
            }
        }

        private async Task<ItemDetail> GenerateItemDetail(Guid workorderID, string serialNo)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShopfloorDBContext>();
                var itemDetail = new ItemDetail(workorderID, serialNo);
                await dbContext.ItemDetails.AddAsync(itemDetail);
                await dbContext.SaveChangesAsync();
                return itemDetail;
            }
        }

        private async Task<RequestResult> UpsertItemDetail(ItemDetail itemDetail)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ShopfloorDBContext>();
                    var target = dbContext.ItemDetails.FirstOrDefault(x => x.Id == itemDetail.Id);
                    if (target != null)
                    {
                        target.WorkordersId = itemDetail.WorkordersId;
                        target.SerialNo = itemDetail.SerialNo;
                        target.TargetAmount = itemDetail.TargetAmount;
                        target.Okamount = itemDetail.Okamount;
                        target.Ngamount = itemDetail.Ngamount;
                        target.StartTime = itemDetail.StartTime;
                        target.FinishedTime = itemDetail.FinishedTime;
                    }
                    else
                    {
                        await dbContext.ItemDetails.AddAsync(itemDetail);
                    }
                    await dbContext.SaveChangesAsync();
                    return new(2, $"Upsert task success");
                }
                catch (Exception ex)
                {
                    return new RequestResult(4, $"Upsert task fail({ex.Message})");
                }
            }
        }

        #endregion

        #region task

        private async Task<TaskDetail> GenerateTaskDetail(Guid itemID, Guid stationID)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShopfloorDBContext>();
                var taskDetail = new TaskDetail(itemID, stationID);
                await dbContext.TaskDetails.AddAsync(taskDetail);
                await dbContext.SaveChangesAsync();
                return taskDetail;
            }
        }

        private async Task<RequestResult> UpsertTaskDetail(TaskDetail taskDetail)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ShopfloorDBContext>();
                    var target = dbContext.TaskDetails.FirstOrDefault(x => x.Id == taskDetail.Id);
                    if (target != null)
                    {
                        target.ItemId = taskDetail.ItemId;
                        target.StationId = taskDetail.StationId;
                        target.StartTime = taskDetail.StartTime;
                        target.FinishedTime = taskDetail.FinishedTime;
                    }
                    else
                    {
                        await dbContext.TaskDetails.AddAsync(taskDetail);
                    }
                    await dbContext.SaveChangesAsync();
                    return new(2, $"Upsert task success");
                }
                catch (Exception ex)
                {
                    return new RequestResult(4, $"Upsert task fail({ex.Message})");
                }
            }
        }

        #endregion
    }
}
