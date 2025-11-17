using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonLibraryP.API;

namespace CommonLibraryP.MachinePKG
{
    public class EquipmentSpecService
    {
        private readonly IServiceScopeFactory scopeFactory;

        public EquipmentSpecService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        // 取得所有 EquipmentSpec
        public Task<List<EquipmentSpec>> GetAllEquipmentSpecs()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                return Task.FromResult(dbContext.EquipmentSpecs.AsNoTracking().ToList());
            }
        }

        // 依 Id 取得單筆 EquipmentSpec
        public Task<EquipmentSpec?> GetEquipmentSpecByIdAsync(int id)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                return Task.FromResult(dbContext.EquipmentSpecs.AsNoTracking().FirstOrDefault(x => x.Id == id));
            }
        }
        public async Task<List<string>> GetAllLineCodesAsync()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                return await dbContext.EquipmentSpecs
                    .AsNoTracking()
                    .Where(x => !string.IsNullOrEmpty(x.線別編號))
                    .Select(x => x.線別編號)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToListAsync();
            }
        }
        // 新增或更新 EquipmentSpec
        public async Task<RequestResult> UpsertEquipmentSpec(EquipmentSpec spec)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                    var target = dbContext.EquipmentSpecs.FirstOrDefault(x =>
                        x.機台編號 == spec.機台編號 &&
                        x.資訊項目 == spec.資訊項目 &&
                        x.機台項目說明 == spec.機台項目說明 &&
                        x.PLC_XY位址 == spec.PLC_XY位址
                    );


                    bool exist = target is not null;
                    if (exist)
                    {
                        // 更新現有記錄，手動複製屬性（不更新 Id）
                        target.項目 = spec.項目;
                        target.機台名稱 = spec.機台名稱;
                        target.機種代碼 = spec.機種代碼;
                        target.機台編號 = spec.機台編號;
                        target.線別編號 = spec.線別編號;
                        target.資訊項目 = spec.資訊項目;
                        target.機台項目說明 = spec.機台項目說明;
                        target.機台項目代碼 = spec.機台項目代碼;
                        target.規格型號 = spec.規格型號;
                        target.說明1 = spec.說明1;
                        target.PLC讀值型態 = spec.PLC讀值型態;
                        target.PLC_XY位址 = spec.PLC_XY位址;
                        target.PLC讀值位址ModbusAdd = spec.PLC讀值位址ModbusAdd;
                        target.條件或格式 = spec.條件或格式;
                        target.電控制箱編號 = spec.電控制箱編號;
                        target.電控制箱IP = spec.電控制箱IP;
                    }
                    else
                    {
                        await dbContext.EquipmentSpecs.AddAsync(spec);
                    }
                    await dbContext.SaveChangesAsync();
                    return new(2, $"Upsert EquipmentSpec success");
                }
                catch (Exception e)
                {
                    return new(4, $"Upsert EquipmentSpec fail({e.Message})");
                }
            }
        }

        // 刪除 EquipmentSpec
        public async Task<RequestResult> DeleteEquipmentSpec(int id)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                    var target = dbContext.EquipmentSpecs.FirstOrDefault(x => x.Id == id);
                    if (target != null)
                    {
                        dbContext.EquipmentSpecs.Remove(target);
                        await dbContext.SaveChangesAsync();
                        return new(2, $"Delete EquipmentSpec {id} success");
                    }
                    else
                    {
                        return new(4, $"EquipmentSpec {id} not found");
                    }
                }
                catch (Exception e)
                {
                    return new(4, $"Delete EquipmentSpec {id} fail({e.Message})");
                }
            }
        }
        public async Task<(bool IsSuccess, string Msg)> DeleteAsync(string machineNo, string item)
        {
            if (string.IsNullOrWhiteSpace(machineNo) || string.IsNullOrWhiteSpace(item))
                return (false, "機台編號與項目不可為空");

            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                var entity = await dbContext.EquipmentSpecs
                    .FirstOrDefaultAsync(x => x.機台編號 == machineNo && x.項目 == item);

                if (entity == null)
                    return (false, "找不到指定資料");

                dbContext.EquipmentSpecs.Remove(entity);
                await dbContext.SaveChangesAsync();
                return (true, "刪除成功");
            }
        }
        // 取得全部設備規格資料
        public async Task<List<EquipmentSpec>> GetAllAsync()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                return await dbContext.EquipmentSpecs.AsNoTracking().ToListAsync();
            }
        }
    }
}