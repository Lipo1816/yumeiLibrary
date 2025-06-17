using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.Service
{
    public class TempratureHuLogService
    {
        private readonly MachineDBContext _db;

        public TempratureHuLogService(MachineDBContext db)
        {
            _db = db;
        }

        // 取得全部
        public async Task<List<temprature_Hu_log>> GetAllAsync()
        {
            return await _db.temprature_Hu_logs.AsNoTracking().ToListAsync();
        }

        // 依 Id 取得單筆
        public async Task<temprature_Hu_log?> GetByIdAsync(int id)
        {
            return await _db.temprature_Hu_logs.FindAsync(id);
        }

        // 新增或更新
        public async Task UpsertAsync(temprature_Hu_log item)
        {
            if (item.Id == 0)
            {
                _db.temprature_Hu_logs.Add(item);
            }
            else
            {
                _db.temprature_Hu_logs.Update(item);
            }
            await _db.SaveChangesAsync();
        }

        // 刪除
        public async Task DeleteAsync(int id)
        {
            var entity = await _db.temprature_Hu_logs.FindAsync(id);
            if (entity != null)
            {
                _db.temprature_Hu_logs.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }
    }
}
