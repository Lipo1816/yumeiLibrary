using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.Service
{
    public class TempratureHuService
    {
        private readonly MachineDBContext _db;

        public TempratureHuService(MachineDBContext db)
        {
            _db = db;
        }

        // 取得全部
        public async Task<List<temprature_Hu>> GetAllAsync()
        {
            return await _db.temprature_Hus.AsNoTracking().ToListAsync();
        }

        // 取得單筆
        public async Task<temprature_Hu?> GetByIdAsync(int id)
        {
            return await _db.temprature_Hus.FindAsync(id);
        }

        // 新增或更新
        public async Task UpsertAsync(temprature_Hu item)
        {
            if (item.Id == 0)
            {
                _db.temprature_Hus.Add(item);
            }
            else
            {
                _db.temprature_Hus.Update(item);
            }
            await _db.SaveChangesAsync();
        }

        // 刪除
        public async Task DeleteAsync(int id)
        {
            var entity = await _db.temprature_Hus.FindAsync(id);
            if (entity != null)
            {
                _db.temprature_Hus.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }
    }
}
