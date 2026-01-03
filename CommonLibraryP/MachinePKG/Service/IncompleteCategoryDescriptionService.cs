using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.Service
{
    public class IncompleteCategoryDescriptionService
    {
        private readonly IDbContextFactory<MachineDBContext> _dbFactory;

        public IncompleteCategoryDescriptionService(IDbContextFactory<MachineDBContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        // 取得全部
        public async Task<List<IncompleteCategoryDescription>> GetAllAsync()
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            return await db.IncompleteCategoryDescriptions
                .OrderBy(x => x.排序順序)
                .ThenBy(x => x.未完成類別)
                .ThenBy(x => x.未完成說明)
                .AsNoTracking()
                .ToListAsync();
        }

        // 取得單筆
        public async Task<IncompleteCategoryDescription?> GetByIdAsync(int id)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            return await db.IncompleteCategoryDescriptions.FindAsync(id);
        }

        // 新增或更新
        public async Task UpsertAsync(IncompleteCategoryDescription item)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            if (item.Id == 0)
            {
                db.IncompleteCategoryDescriptions.Add(item);
            }
            else
            {
                db.IncompleteCategoryDescriptions.Update(item);
            }
            await db.SaveChangesAsync();
        }

        // 刪除
        public async Task DeleteAsync(int id)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            var entity = await db.IncompleteCategoryDescriptions.FindAsync(id);
            if (entity != null)
            {
                db.IncompleteCategoryDescriptions.Remove(entity);
                await db.SaveChangesAsync();
            }
        }
    }
}
