using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonLibraryP.API;
using CommonLibraryP.MachinePKG.EFModel;

namespace CommonLibraryP.MachinePKG.Service
{
    public class PersonnalService
    {
        private readonly IServiceScopeFactory scopeFactory;

        public PersonnalService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        // 取得所有 Personnal
        public Task<List<Personnal>> GetAllPersonnals()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                return Task.FromResult(dbContext.Personnal.AsNoTracking().ToList());
            }
        }
        public Task<List<string>> GetAllProductionGroupsAsync()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                var groups = dbContext.Personnal
                    .AsNoTracking()
                    .Where(x => !string.IsNullOrEmpty(x.生產組名))
                    .Select(x => x.生產組名)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
                return Task.FromResult(groups);
            }
        }

        public async Task<List<Personnal>> GetAllAsync()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                return await dbContext.Personnal.ToListAsync();
            }
        }
        // 依 人員ID 取得單筆 Personnal
        public Task<Personnal?> GetPersonnalByIdAsync(string id)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                return Task.FromResult(dbContext.Personnal.AsNoTracking().FirstOrDefault(x => x.人員ID == id));
            }
        }

        // 新增或更新 Personnal
        public async Task<RequestResult> UpsertPersonnal(Personnal p)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                    var target = dbContext.Personnal.FirstOrDefault(x => x.人員ID == p.人員ID);
                    bool exist = target is not null;
                    if (exist)
                    {
                        dbContext.Entry(target).CurrentValues.SetValues(p);
                    }
                    else
                    {
                        await dbContext.Personnal.AddAsync(p);
                    }
                    await dbContext.SaveChangesAsync();
                    return new(2, $"Upsert Personnal {p.人員ID} success");
                }
                catch (Exception e)
                {
                    return new(4, $"Upsert Personnal {p.人員ID} fail({e.Message})");
                }
            }
        }

        // 刪除 Personnal
        public async Task<RequestResult> DeletePersonnal(string id)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                    var target = dbContext.Personnal.FirstOrDefault(x => x.人員ID == id);
                    if (target != null)
                    {
                        dbContext.Personnal.Remove(target);
                        await dbContext.SaveChangesAsync();
                        return new(2, $"Delete Personnal {id} success");
                    }
                    else
                    {
                        return new(4, $"Personnal {id} not found");
                    }
                }
                catch (Exception e)
                {
                    return new(4, $"Delete Personnal {id} fail({e.Message})");
                }
            }
        }

        // 複製 Personnal（Copy）
        public async Task<RequestResult> CopyPersonnal(string id, string newId)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                    var target = dbContext.Personnal.AsNoTracking().FirstOrDefault(x => x.人員ID == id);
                    if (target != null)
                    {
                        var copy = new Personnal
                        {
                            人員ID = newId,
                            部門ID = target.部門ID,
                            部門名稱 = target.部門名稱,
                            生產組名 = target.生產組名,
                            人員姓名 = target.人員姓名,
                            職級代號 = target.職級代號,
                            職級名稱 = target.職級名稱,
                            Email = target.Email,
                            權限 = target.權限,
                            權限頁面 = target.權限頁面
                        };
                        await dbContext.Personnal.AddAsync(copy);
                        await dbContext.SaveChangesAsync();
                        return new(2, $"Copy Personnal {id} to {newId} success");
                    }
                    else
                    {
                        return new(4, $"Personnal {id} not found");
                    }
                }
                catch (Exception e)
                {
                    return new(4, $"Copy Personnal {id} fail({e.Message})");
                }
            }
        }

        // 剪下 Personnal（Cut = Copy + Delete）
        public async Task<RequestResult> CutPersonnal(string id, string newId)
        {
            var copyResult = await CopyPersonnal(id, newId);
            if (copyResult.IsSuccess)
            {
                return await DeletePersonnal(id);
            }
            return copyResult;
        }

    }
}