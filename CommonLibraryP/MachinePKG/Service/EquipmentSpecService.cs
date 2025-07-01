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
                    var target = dbContext.EquipmentSpecs.FirstOrDefault(x => x.Id == spec.Id);
                    bool exist = target is not null;
                    if (exist)
                    {
                        dbContext.Entry(target).CurrentValues.SetValues(spec);
                    }
                    else
                    {
                        await dbContext.EquipmentSpecs.AddAsync(spec);
                    }
                    await dbContext.SaveChangesAsync();
                    return new(2, $"Upsert EquipmentSpec {spec.Id} success");
                }
                catch (Exception e)
                {
                    return new(4, $"Upsert EquipmentSpec {spec.Id} fail({e.Message})");
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