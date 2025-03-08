using CommonLibraryP.API;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CommonLibraryP.Data;
using System.Net;

namespace CommonLibraryP.MachinePKG
{
    public class MachineService
    {
        private readonly IServiceScopeFactory scopeFactory;
        public MachineService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
            //IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            //var addr = ipEntry.AddressList.Where(x=>x.AddressFamily== System.Net.Sockets.AddressFamily.InterNetwork);
        }

        #region modbus slave

        private List<ModbusSlaveConfig> modbusSlaves = new();
        public List<ModbusSlaveConfig> ModbusSlaves => modbusSlaves;
        public async Task InitAllModbusSlaves()
        {
            modbusSlaves = await GetAllModbusSlaveConfigs();
            foreach (var slave in modbusSlaves)
            {
                try
                {
                    await slave.Init();
                }
                catch (Exception e)
                {

                }
            }
        }
        public Task<List<ModbusSlaveConfig>> GetAllModbusSlaveConfigs()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                return Task.FromResult(dbContext.ModbusSlaveConfigs.AsNoTracking().ToList());
            }
        }
        public async Task<RequestResult> UpsertMudbusConfig(ModbusSlaveConfig modbusSlaveConfig)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                    var target = dbContext.ModbusSlaveConfigs.FirstOrDefault(x => x.Id == modbusSlaveConfig.Id);
                    bool exist = target is not null;
                    if (exist)
                    {
                        target.Ip = modbusSlaveConfig.Ip;
                        target.Port = modbusSlaveConfig.Port;
                        target.Station = modbusSlaveConfig.Station;
                    }
                    else
                    {
                        await dbContext.ModbusSlaveConfigs.AddAsync(modbusSlaveConfig);
                    }
                    await dbContext.SaveChangesAsync();
                    return new(2, $"upsert modbus slave {modbusSlaveConfig.Ip} success");
                }
                catch (Exception e)
                {
                    return new(4, $"upsert modbus slave {modbusSlaveConfig.Ip} fail({e.Message})");
                }

            }
        }

        #endregion

        #region machine
        private List<Machine> machines = new();

        public Action<Guid, DataEditMode>? MachineConfigChangedAct { get; set; }

        public List<Machine> Machines => machines;

        public Task<List<Machine>> GetAllMachinesConfig()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                return Task.FromResult(dbContext.Machines.AsNoTracking().ToList());
            }
        }

        //public virtual IEnumerable<ConnectionTypeWrapperClass> GetConnectTypesWrapperClass() { }

        public async Task InitAllMachinesFromDB()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                var tmp = dbContext.Machines.Include(x => x.TagCategory).ThenInclude(x => x.Tags)
                    .AsSplitQuery()
                    .AsNoTracking()
                    .ToList();
                machines = tmp.Select(x => InitMachineToDerivesClass(x)).ToList();
                List<Task> tasks = new();
                foreach (Machine machine in machines)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        machine.InitMachine();
                        if (machine.Enabled)
                        {
                            machine.StartUpdating();
                        }
                    }));

                }
                await Task.WhenAll(tasks);
            }
        }

        public Machine? InitMachineFromDBById(Guid id)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                var tmp = dbContext.Machines.Include(x => x.TagCategory).ThenInclude(x => x.Tags)
                    .AsSplitQuery()
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == id);
                tmp = InitMachineToDerivesClass(tmp);
                tmp.InitMachine();
                if (tmp.Enabled)
                {
                    tmp.StartUpdating();
                }
                return tmp;
            }
        }

        public async Task<RequestResult> UpsertMachineConfig(Machine machine)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                    var target = dbContext.Machines.FirstOrDefault(x => x.Id == machine.Id);
                    bool exist = target is not null;
                    if (exist)
                    {
                        target.Name = machine.Name;
                        //target.ProcessId = machine.ProcessId;
                        target.Ip = machine.Ip;
                        target.Port = machine.Port;
                        target.ConnectionType = machine.ConnectionType;
                        target.MaxRetryCount = machine.MaxRetryCount;
                        target.TagCategoryId = machine.TagCategoryId;
                        //target.LogicStatusCategoryId = machine.LogicStatusCategoryId;
                        //target.ErrorCodeCategoryId = machine.ErrorCodeCategoryId;
                        target.Enabled = machine.Enabled;
                        target.UpdateDelay = machine.UpdateDelay;
                        target.RecordStatusChanged = machine.RecordStatusChanged;
                    }
                    else
                    {
                        await dbContext.Machines.AddAsync(machine);
                    }
                    await dbContext.SaveChangesAsync();
                    DataEditMode dataEditMode = exist ? DataEditMode.Update : DataEditMode.Insert;
                    await RefreshMachine(machine, dataEditMode);
                    return new(2, $"upsert machine {machine.Name} success");
                }
                catch (Exception e)
                {
                    return new(4, $"upsert machine {machine.Name} fail({e.Message})");
                }

            }
        }

        public async Task<RequestResult> DeleteMachine(Machine machine)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                    var target = dbContext.Machines.FirstOrDefault(x => x.Id == machine.Id);
                    if (target != null)
                    {
                        dbContext.Remove(target);
                        await dbContext.SaveChangesAsync();
                        await RefreshMachine(target, DataEditMode.Delete);
                        return new(2, $"Delete machine {machine.Name} success");
                    }
                    else
                    {
                        return new(4, $"Machine {machine.Name} not found");
                    }

                }
                catch (Exception e)
                {
                    return new(4, $"Delete machine {machine.Name} fail({e.Message})");
                }

            }
        }

        public Task<Machine?> GetMachineByID(Guid? id)
        {
            return Task.FromResult(machines.FirstOrDefault(x => x.Id == id));
        }

        public Task<Machine?> GetMachineByName(string name)
        {
            return Task.FromResult(machines.FirstOrDefault(x => x.Name == name));
        }

        public virtual Machine InitMachineToDerivesClass(Machine machine)
        {
            Machine res;
            switch (machine.ConnectionType)
            {
                case 0:
                    res = new ModbusTCPMachine(machine);
                    break;
                case 1:
                    res = new TMRobotModbusTCP(machine);
                    break;
                default:
                    throw new NotImplementedException();
            }
            res.MachineStatechangedRecordAct += MachineStatusChangedRecord;
            return res;
        }

        public void MachineConfigChanged(Guid id, DataEditMode mode)
        {
            MachineConfigChangedAct?.Invoke(id, mode);
        }

        public async Task RefreshMachine(Machine machine, DataEditMode dataEditMode)
        {
            var target = await GetMachineByID(machine.Id);
            if (target != null)
            {
                //update or delete
                target.MachineStatechangedRecordAct += MachineStatusChangedRecord;
                machines.Remove(target);
                target.Dispose();

                if (dataEditMode != DataEditMode.Delete)
                {
                    machines.Add(InitMachineFromDBById(machine.Id));
                }
                else
                {
                }
            }
            else
            {
                machines.Add(InitMachineFromDBById(machine.Id));
            }
            MachineConfigChanged(machine.Id, dataEditMode);
        }

        public async void MachineStatusChangedRecord(Machine machine, MachineStatusRecordType machineStatusRecordType)
        {
            if (!machine.RecordStatusChanged)
            {
                return;
            }
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                    var newRocord = new MachineStatusLog
                    {
                        Id = Guid.NewGuid(),
                        MachineID = machine.Id,
                        Status = (int)machine.MachineStatus,
                        LogTime = DateTime.Now,
                    };
                    await dbContext.MachineStatusLogs.AddAsync(newRocord);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public RequestResult RegisterTagValueChange(string machineName, string tagName, Action<Tag> tagListener)
        {
            var targetMachine = machines.FirstOrDefault(x => x.Name == machineName);
            if (targetMachine is null)
            {
                return new(4, $"Machine {machineName} not found");
            }
            var targetTag = targetMachine.TagCategory?.Tags.FirstOrDefault(x => x.Name == tagName);
            if (targetTag is null)
            {
                return new(4, $"Tag {tagName} not found in machine {machineName}");
            }

            tagListener += targetTag.TagValueChanged;
            return new(2, $"Listen machine {machineName} tag {tagName} value change success");
        }

        #endregion

        #region utilization

        public Task<List<MachineStatusLog>> GetMachineStatusLogByID(MachineUtilizationDTO machineUtilizationDTO)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                return Task.FromResult(dbContext.MachineStatusLogs.Where(x => x.MachineID == machineUtilizationDTO.MachineID && x.LogTime >= machineUtilizationDTO.Start && x.LogTime <= machineUtilizationDTO.End).OrderBy(x => x.LogTime).AsNoTracking().ToList());
            }
        }

        public async IAsyncEnumerable<MachineStatusInterval> CalculateMachineStatusIntervalByOrderedLog(List<MachineStatusLog> machineStatusLogs, ushort delayMilliSec, IProgress<int>? progress)
        {
            int totalCount = machineStatusLogs.Count();
            progress?.Report(0);
            for (int i = 0; i < totalCount; i++)
            {
                if (i == totalCount - 1)
                {
                    //res.Add(new(machineStatusLogs[i].LogTime, DateTime.Now, (Status)machineStatusLogs[i].Status));
                    yield return new(machineStatusLogs[i].LogTime, DateTime.Now, (Status)machineStatusLogs[i].Status);
                }
                else
                {
                    //res.Add(new(machineStatusLogs[i].LogTime, machineStatusLogs[i + 1].LogTime, (Status)machineStatusLogs[i].Status));
                    yield return new(machineStatusLogs[i].LogTime, machineStatusLogs[i + 1].LogTime, (Status)machineStatusLogs[i].Status);
                }
                await Task.Delay(delayMilliSec);
                progress?.Report(i * 100 / totalCount);
            }
        }

        public Task<RequestResult> ClearMachineStatusLogBeforeSpecificTime(DateTime? time)
        {
            var t = time is null ? DateTime.Now : time.Value;
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                    var targets = dbContext.MachineStatusLogs.Where(x => x.LogTime < t);
                    if (targets.Count() > 0)
                    {
                        dbContext.MachineStatusLogs.RemoveRange(targets);
                        dbContext.SaveChanges();
                        return Task.FromResult(new RequestResult(2, $"Clear machine status log before {t} success"));
                    }
                    else
                    {
                        return Task.FromResult(new RequestResult(1, $"No machine status logs before {t}"));
                    }
                }
                catch (Exception ex)
                {
                    return Task.FromResult(new RequestResult(4, ex.Message));
                }
            }
        }

        #endregion

        #region tag
        public Task<List<TagCategory>> GetAllTagCategories()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                return Task.FromResult(dbContext.TagCategories.AsNoTracking().ToList());
            }
        }

        public Task<List<TagCategory>> GetAllTagCategoriesWithTags()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                return Task.FromResult(dbContext.TagCategories.Include(x => x.Tags).AsNoTracking().ToList());
            }
        }

        public List<Tag> GetTagsByCatId(Guid? catID)
        {
            if (catID is null)
            {
                return new List<Tag>();
            }
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                var targetCat = dbContext.TagCategories.Include(x => x.Tags).AsNoTracking().FirstOrDefault(x => x.Id == catID);
                if (targetCat is not null)
                {
                    return targetCat.Tags.ToList();
                }
                else
                {
                    return new List<Tag>();
                }
            }
        }

        public int GetTagTypeCodeByIds(Guid? catID, Guid? tagID)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                var targetTag = dbContext.Tags.FirstOrDefault(x => x.CategoryId == catID && x.Id == tagID);
                return targetTag is null ? 0 : targetTag.DataType;
            }
        }

        public Task<List<TagCategory>> GetCategoryByConnectionType(int connectionType)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                return Task.FromResult(dbContext.TagCategories.Where(x => x.ConnectionType == connectionType).ToList());
            }
        }

        public async Task<RequestResult> UpsertTagCategory(TagCategory tagCategory)
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                    var targetTagCat = dbContext.TagCategories.FirstOrDefault(x => x.Id == tagCategory.Id);
                    if (targetTagCat != null)
                    {
                        targetTagCat.Name = tagCategory.Name;
                        targetTagCat.ConnectionType = tagCategory.ConnectionType;
                    }
                    else
                    {
                        await dbContext.TagCategories.AddAsync(tagCategory);
                    }
                    await dbContext.SaveChangesAsync();
                    return new(2, $"Upsert tag category {tagCategory.Name} success");
                }
            }
            catch (Exception ex)
            {
                return new(4, ex.Message);
            }
        }

        public async Task<RequestResult> DeleteTagCategory(TagCategory tagCategory)
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                    var targetTagCat = dbContext.TagCategories.Include(x => x.Tags).FirstOrDefault(x => x.Id == tagCategory.Id);
                    if (targetTagCat != null)
                    {
                        dbContext.TagCategories.Remove(targetTagCat);
                        await dbContext.SaveChangesAsync();
                        return new(2, $"Delete tag category {targetTagCat.Name} success");
                    }
                    else
                    {
                        return new(4, $"Tag category {targetTagCat.Name} not found");
                    }

                }
            }
            catch (Exception ex)
            {
                return new(4, ex.Message);
            }
        }

        public async Task<RequestResult> UpsertTag(Tag tag)
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                    var targetTag = dbContext.Tags.FirstOrDefault(x => x.Id == tag.Id);
                    if (targetTag != null)
                    {
                        targetTag.Name = tag.Name;
                        targetTag.DataType = tag.DataType;
                        targetTag.UpdateByTime = tag.UpdateByTime;
                        targetTag.SpecialType = tag.SpecialType;

                        targetTag.Bool1 = tag.Bool1;
                        targetTag.Bool2 = tag.Bool2;
                        targetTag.Bool3 = tag.Bool3;
                        targetTag.Bool4 = tag.Bool4;
                        targetTag.Bool5 = tag.Bool5;

                        targetTag.Int1 = tag.Int1;
                        targetTag.Int2 = tag.Int2;
                        targetTag.Int3 = tag.Int3;
                        targetTag.Int4 = tag.Int4;
                        targetTag.Int5 = tag.Int5;

                        targetTag.String1 = tag.String1;
                        targetTag.String2 = tag.String2;
                        targetTag.String3 = tag.String3;
                        targetTag.String4 = tag.String4;
                        targetTag.String5 = tag.String5;
                    }
                    else
                    {
                        await dbContext.Tags.AddAsync(tag);
                    }
                    await dbContext.SaveChangesAsync();
                    return new(2, $"Upsert tag {tag.Name} success");
                }
            }
            catch (Exception ex)
            {
                return new(4, ex.Message);
            }
        }

        public async Task<RequestResult> DeleteTag(Tag tag)
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                    var targetTag = dbContext.Tags.FirstOrDefault(x => x.Id == tag.Id);
                    if (targetTag != null)
                    {
                        dbContext.Tags.Remove(targetTag);
                        await dbContext.SaveChangesAsync();
                        return new(2, $"Delete tag {tag.Name} success");
                    }
                    else
                    {
                        return new(4, $"Tag {tag.Name} not found");
                    }

                }
            }
            catch (Exception ex)
            {
                return new(4, ex.Message);
            }
        }

        public async Task<Tag?> GetMachineTag(string machineName, string tagName)
        {
            Machine? targetMachine = await GetMachineByName(machineName);
            if (targetMachine != null)
            {
                if (targetMachine.hasCategory)
                {
                    Tag? targetTag = targetMachine.TagCategory.Tags.FirstOrDefault(x => x.Name == tagName);
                    if (targetTag != null)
                    {
                        if (!targetTag.UpdateByTime)
                        {
                            await targetMachine.UpdateTag(targetTag);
                        }
                        return targetTag;
                    }
                }
            }
            return null;
        }

        public async Task<RequestResult> SetMachineTag(string machineName, string tagName, object val)
        {
            Machine? targetMachine = await GetMachineByName(machineName);
            if (targetMachine != null)
            {
                if (targetMachine.hasCategory)
                {
                    Tag? targetTag = targetMachine.TagCategory.Tags.FirstOrDefault(x => x.Name == tagName);
                    if (targetTag != null)
                    {
                        return await targetMachine.SetTag(targetTag.Name, val);
                    }
                    else
                    {
                        return new(4, $"Tag {tagName} not found in machine {machineName}");
                    }
                }
                else
                {
                    return new(4, $"Machine tag category not set");
                }
            }
            else
            {
                return new(4, $"Machine {machineName} not found");
            }
        }


        #endregion

        #region condition

        private List<Condition> conditions = new();
        public List<Condition> Conditions => conditions;
        public async Task InitConditions()
        {
            conditions = await GetAllConditions();
        }

        public async Task<List<Condition>> GetAllConditions()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                return await dbContext.Conditions.Include(x => x.ConditionRootNode)
                    .ThenInclude(x => x.ChildrenNodes)
                    .AsNoTracking()
                    .ToListAsync();
            }
        }
        public async Task<RequestResult> UpsertCondition(Condition condition)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                    var target = dbContext.Conditions.FirstOrDefault(x => x.Id == condition.Id);
                    bool exist = target is not null;
                    if (exist)
                    {
                        target.Name = condition.Name;
                        target.Enable = condition.Enable;
                    }
                    else
                    {
                        await dbContext.Conditions.AddAsync(condition);
                    }
                    await dbContext.SaveChangesAsync();
                    //DataEditMode dataEditMode = exist ? DataEditMode.Update : DataEditMode.Insert;
                    //await RefreshMachine(machine, dataEditMode);
                    return new(2, $"upsert condition {condition.Name} success");
                }
                catch (Exception e)
                {
                    return new(4, $"upsert condition {condition.Name} fail({e.Message})");
                }

            }
        }
        public async Task<RequestResult> DeleteCondition(Condition condition)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                    var target = dbContext.Conditions.FirstOrDefault(x => x.Id == condition.Id);
                    if (target != null)
                    {
                        dbContext.Remove(target);
                        await dbContext.SaveChangesAsync();
                        return new(2, $"Delete condition {target.Name} success");
                    }
                    else
                    {
                        return new(4, $"condition {condition.Name} not found");
                    }

                }
                catch (Exception e)
                {
                    return new(4, $"Delete condition {condition.Name} fail({e.Message})");
                }

            }
        }

        public async Task<RequestResult> UpsertConditionNode(ConditionNode conditionNode)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                    var target = dbContext.ConditionNodes.FirstOrDefault(x => x.Id == conditionNode.Id);
                    bool exist = target is not null;
                    if (exist)
                    {
                        target.ConditionId = conditionNode.ConditionId;
                        target.ParentNodeId = conditionNode.ParentNodeId;
                        target.LeafPosition = conditionNode.LeafPosition;
                        target.MachineId = conditionNode.MachineId;
                        target.TagId = conditionNode.TagId;
                        target.LogicalOperation = conditionNode.LogicalOperation;
                    }
                    else
                    {
                        await dbContext.ConditionNodes.AddAsync(conditionNode);
                    }
                    await dbContext.SaveChangesAsync();
                    return new(2, $"upsert condition node success");
                }
                catch (Exception e)
                {
                    return new(4, $"upsert condition condition node fail({e.Message})");
                }

            }
        }

        public async Task<RequestResult> DeleteConditionNode(ConditionNode conditionNode)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                    var target = dbContext.ConditionNodes.FirstOrDefault(x => x.Id == conditionNode.Id);
                    if (target != null)
                    {
                        dbContext.Remove(target);
                        await dbContext.SaveChangesAsync();
                        return new(2, $"Delete condition node success");
                    }
                    else
                    {
                        return new(4, $"condition node not found");
                    }

                }
                catch (Exception e)
                {
                    return new(4, $"Delete condition node fail({e.Message})");
                }

            }
        }

        #endregion
    }
}
