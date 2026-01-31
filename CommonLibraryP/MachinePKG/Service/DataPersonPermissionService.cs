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
    public class DataPersonPermissionService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DataPersonPermissionService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        // 取得全部
        public async Task<List<Data_Person_Permissions>> GetAllAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            return await db.DataPersonPermissions.AsNoTracking().ToListAsync();
        }

        // 依 Id 取得單筆
        public async Task<Data_Person_Permissions?> GetByIdAsync(int id)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            return await db.DataPersonPermissions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        // 新增或更新
        public async Task<(bool IsSuccess, string Msg)> UpsertAsync(Data_Person_Permissions data)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            try
            {
                var target = await db.DataPersonPermissions.FirstOrDefaultAsync(x => x.Id == data.Id);
                if (target != null)
                {
                    db.Entry(target).CurrentValues.SetValues(data);
                }
                else
                {
                    await db.DataPersonPermissions.AddAsync(data);
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
                var target = await db.DataPersonPermissions.FirstOrDefaultAsync(x => x.Id == id);
                if (target != null)
                {
                    db.DataPersonPermissions.Remove(target);
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

        /// <summary>刪除不在指定人員清單中的權限記錄（Excel 完全取代匯入時同步）</summary>
        public async Task<(bool IsSuccess, int DeletedCount)> DeleteWherePersonNotInListAsync(IEnumerable<string> keepPersonIds)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            try
            {
                var keepSet = new HashSet<string>(keepPersonIds.Select(x => x?.Trim() ?? "").Where(x => !string.IsNullOrEmpty(x)), StringComparer.OrdinalIgnoreCase);
                var toDelete = await db.DataPersonPermissions.Where(x => x.人員ID != null && !keepSet.Contains(x.人員ID)).ToListAsync();
                var count = toDelete.Count;
                if (count > 0)
                {
                    db.DataPersonPermissions.RemoveRange(toDelete);
                    await db.SaveChangesAsync();
                }
                return (true, count);
            }
            catch
            {
                return (false, 0);
            }
        }

        /// <summary>依人員ID刪除權限記錄（人員刪除時同步）</summary>
        public async Task<(bool IsSuccess, string Msg)> DeleteByPersonIdAsync(string personId)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            try
            {
                var targets = await db.DataPersonPermissions.Where(x => x.人員ID == personId).ToListAsync();
                if (targets.Any())
                {
                    db.DataPersonPermissions.RemoveRange(targets);
                    await db.SaveChangesAsync();
                }
                return (true, "刪除成功");
            }
            catch (Exception ex)
            {
                return (false, $"刪除失敗: {ex.Message}");
            }
        }

        /// <summary>確保人員有權限記錄，若無則新增（全無權限）</summary>
        public async Task<(bool IsSuccess, string Msg)> EnsurePermissionForPersonAsync(string 人員ID, string? 人員姓名, string? 生產組名)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            try
            {
                var exists = await db.DataPersonPermissions.AnyAsync(x => x.人員ID == 人員ID);
                if (!exists)
                {
                    var newPerm = new Data_Person_Permissions
                    {
                        人員ID = 人員ID,
                        人員姓名 = 人員姓名 ?? "",
                        生產組名 = 生產組名 ?? "",
                        工單看板 = "0", 設備看板 = "0", 品管看板 = "0",
                        設備管理 = "0", 設備點檢 = "0", 工單報工 = "0",
                        人員 = "0", 資料分析 = "0", 資料設定 = "0", 管理設定 = "0"
                    };
                    await db.DataPersonPermissions.AddAsync(newPerm);
                    await db.SaveChangesAsync();
                }
                return (true, "OK");
            }
            catch (Exception ex)
            {
                return (false, $"確保權限失敗: {ex.Message}");
            }
        }

        /// <summary>人員ID變更時：刪除舊權限，為新人員ID新增全無權限記錄</summary>
        public async Task<(bool IsSuccess, string Msg)> SyncPersonIdChangeAsync(string oldPersonId, string newPersonId, string? 人員姓名, string? 生產組名)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            try
            {
                var oldRecords = await db.DataPersonPermissions.Where(x => x.人員ID == oldPersonId).ToListAsync();
                if (oldRecords.Any())
                {
                    db.DataPersonPermissions.RemoveRange(oldRecords);
                    await db.SaveChangesAsync();
                }
                var existsNew = await db.DataPersonPermissions.AnyAsync(x => x.人員ID == newPersonId);
                if (!existsNew)
                {
                    await db.DataPersonPermissions.AddAsync(new Data_Person_Permissions
                    {
                        人員ID = newPersonId,
                        人員姓名 = 人員姓名 ?? "",
                        生產組名 = 生產組名 ?? "",
                        工單看板 = "0", 設備看板 = "0", 品管看板 = "0",
                        設備管理 = "0", 設備點檢 = "0", 工單報工 = "0",
                        人員 = "0", 資料分析 = "0", 資料設定 = "0", 管理設定 = "0"
                    });
                    await db.SaveChangesAsync();
                }
                return (true, "OK");
            }
            catch (Exception ex)
            {
                return (false, $"同步權限失敗: {ex.Message}");
            }
        }

        /// <summary>更新權限表中的人員姓名、生產組名（人員資料修改時同步）</summary>
        public async Task<(bool IsSuccess, string Msg)> UpdatePersonInfoAsync(string 人員ID, string? 人員姓名, string? 生產組名)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            try
            {
                var target = await db.DataPersonPermissions.FirstOrDefaultAsync(x => x.人員ID == 人員ID);
                if (target != null)
                {
                    target.人員姓名 = 人員姓名 ?? "";
                    target.生產組名 = 生產組名 ?? "";
                    await db.SaveChangesAsync();
                }
                return (true, "OK");
            }
            catch (Exception ex)
            {
                return (false, $"更新失敗: {ex.Message}");
            }
        }

        /// <summary>清空權限表後，從 Personnal 重新建立人員名單，格式為「人名(id)」，權限全為未勾選(0)。aric/lipo/lipo1 不納入。</summary>
        public async Task<(bool IsSuccess, string Msg)> ReplaceAllFromPersonnalAsync(IEnumerable<Personnal> personnals)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            try
            {
                var reserved = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "aric", "lipo", "lipo1" };
                var all = await db.DataPersonPermissions.ToListAsync();
                db.DataPersonPermissions.RemoveRange(all);
                await db.SaveChangesAsync();

                foreach (var p in personnals)
                {
                    if (string.IsNullOrWhiteSpace(p.人員ID) || reserved.Contains(p.人員ID)) continue;
                    await db.DataPersonPermissions.AddAsync(new Data_Person_Permissions
                    {
                        人員ID = p.人員ID,
                        人員姓名 = p.人員姓名 ?? "",
                        生產組名 = p.生產組名 ?? "",
                        工單看板 = "0", 設備看板 = "0", 品管看板 = "0",
                        設備管理 = "0", 設備點檢 = "0", 工單報工 = "0",
                        人員 = "0", 資料分析 = "0", 資料設定 = "0", 管理設定 = "0"
                    });
                }
                await db.SaveChangesAsync();
                return (true, "已從人員資料重新建立權限名單，權限已全部清除為未勾選。");
            }
            catch (Exception ex)
            {
                return (false, $"重新建立失敗: {ex.Message}");
            }
        }

        // 依人員ID取得單筆
        public async Task<Data_Person_Permissions?> GetByPersonIdAsync(string personId)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            return await db.DataPersonPermissions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.人員ID == personId);
        }
    }
}
