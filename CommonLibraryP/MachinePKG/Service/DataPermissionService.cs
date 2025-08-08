using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.Service
{
    public class DataPermissionService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DataPermissionService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        // 取得全部
        public async Task<List<Data_Permission>> GetAllAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            return await db.Data_Permissions.AsNoTracking().ToListAsync();
        }

        // 依 Id 取得單筆
        public async Task<Data_Permission?> GetByIdAsync(int id)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            return await db.Data_Permissions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        // 新增或更新
        public async Task<(bool IsSuccess, string Msg)> UpsertAsync(Data_Permission data)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            try
            {
                var target = await db.Data_Permissions.FirstOrDefaultAsync(x => x.Id == data.Id);
                if (target != null)
                {
                    db.Entry(target).CurrentValues.SetValues(data);
                }
                else
                {
                    await db.Data_Permissions.AddAsync(data);
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
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            try
            {
                var target = await db.Data_Permissions.FirstOrDefaultAsync(x => x.Id == id);
                if (target != null)
                {
                    db.Data_Permissions.Remove(target);
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
        public async Task<Data_Permission?> GetByGroupAsync(string groupName)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            return await db.Data_Permissions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.群組 == groupName);
        }
    }
}
