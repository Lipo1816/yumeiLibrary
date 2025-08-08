using CommonLibraryP.API;
using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.Service
{
    public class EquipmentSpecLimitService
    {
        private readonly IServiceScopeFactory scopeFactory;

        public EquipmentSpecLimitService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        // 取得所有資料
        public async Task<List<EquipmentSpecLimit>> GetAllAsync()
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            return await db.EquipmentSpecLimits.AsNoTracking().ToListAsync();
        }

        // 依 Id 取得單筆
        public async Task<EquipmentSpecLimit?> GetByIdAsync(int id)
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            return await db.EquipmentSpecLimits.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        // 新增或更新
        public async Task<(bool IsSuccess, string Msg)> UpsertAsync(EquipmentSpecLimit data)
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            try
            {
                var target = await db.EquipmentSpecLimits.FirstOrDefaultAsync(x => x.Id == data.Id);
                if (target != null)
                {
                    db.Entry(target).CurrentValues.SetValues(data);
                }
                else
                {
                    await db.EquipmentSpecLimits.AddAsync(data);
                }
                await db.SaveChangesAsync();
                return (true, "儲存成功");
            }
            catch (Exception ex)
            {
                return (false, $"儲存失敗: {ex.Message}");
            }
        }

        // 刪除
        public async Task<(bool IsSuccess, string Msg)> DeleteAsync(int id)
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            try
            {
                var target = await db.EquipmentSpecLimits.FirstOrDefaultAsync(x => x.Id == id);
                if (target != null)
                {
                    db.EquipmentSpecLimits.Remove(target);
                    await db.SaveChangesAsync();
                    return (true, "刪除成功");
                }
                else
                {
                    return (false, "找不到資料");
                }
            }
            catch (Exception ex)
            {
                return (false, $"刪除失敗: {ex.Message}");
            }
        }

        public async Task<RequestResult> UpsertEquipmentSpec(EquipmentSpecLimit spec)
        {
            using var scope = scopeFactory.CreateScope();
            try
            {
                var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                var target = db.EquipmentSpecLimits.FirstOrDefault(x => x.Id == spec.Id);
                bool exist = target is not null;
                if (exist)
                {
                    db.Entry(target).CurrentValues.SetValues(spec);
                }
                else
                {
                    await db.EquipmentSpecLimits.AddAsync(spec);
                }
                await db.SaveChangesAsync();
                return new(2, $"Upsert EquipmentSpecLimit {spec.Id} success");
            }
            catch (Exception e)
            {
                return new(4, $"Upsert EquipmentSpecLimit {spec.Id} fail({e.Message})");
            }
        }
        public async Task<(bool IsSuccess, string Msg)> DeleteAsync(string machineNo, string item)
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            try
            {
                var entity = await db.EquipmentSpecLimits
                    .FirstOrDefaultAsync(x => x.機台編號 == machineNo && x.項目 == item);

                if (entity == null)
                    return (false, "找不到指定資料");

                db.EquipmentSpecLimits.Remove(entity);
                await db.SaveChangesAsync();
                return (true, "刪除成功");
            }
            catch (Exception ex)
            {
                return (false, $"刪除失敗：{ex.Message}");
            }
        }
        public Task<List<EquipmentSpecLimit>> GetAllEquipmentSpecs()
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            return Task.FromResult(db.EquipmentSpecLimits.AsNoTracking().ToList());
        }
        public class TagLimitInfo
        {
            public string MachineCode { get; set; }
            public string TagName { get; set; }
            public double? UpperLimit { get; set; }
            public double? LowerLimit { get; set; }
            // ... 其他欄位
        }

        // EquipmentSpecLimitService
        public async Task<List<TagLimitInfo>> GetAllWithLimitsAsync()
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();

            var query = db.EquipmentSpecLimits
                .Where(x =>
                    x.電壓上限 != null || x.電壓下限 != null ||
                    x.電流上限 != null || x.電流下限 != null ||
                    x.頻率上限 != null || x.頻率下限 != null ||
                    x.轉速上限 != null || x.轉速下限 != null ||
                    x.水溫上限 != null || x.水溫下限 != null
                )
                .Select(x => new TagLimitInfo
                {
                    MachineCode = x.機台編號,
                    TagName = x.項目, // 這裡用 x.項目 當作 TagName，請依實際需求調整
                    UpperLimit = (double?)
                        (x.電壓上限 ?? x.電流上限 ?? x.頻率上限 ?? x.轉速上限 ?? x.水溫上限),
                    LowerLimit = (double?)
                        (x.電壓下限 ?? x.電流下限 ?? x.頻率下限 ?? x.轉速下限 ?? x.水溫下限)
                });

            return await query.ToListAsync();
        }
    }
}
