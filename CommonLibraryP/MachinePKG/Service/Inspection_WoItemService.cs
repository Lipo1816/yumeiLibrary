using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.Service
{
    public class Inspection_WoItemService
    {
        private readonly MachineDBContext _context;

        public Inspection_WoItemService(MachineDBContext context)
        {
            _context = context;
        }
        public async Task<List<Inspection_WoItem>> GetByInspectionNoAsync(string inspectionNo)
        {

            if (_context.Inspection_WoItem == null)
            {
                return null;
            }
            else
            {

                return await _context.Inspection_WoItem
                    .Where(x => x.點檢單號 == inspectionNo)
                    .ToListAsync();
            }
        }
        // Create
        public async Task AddAsync(Inspection_WoItem item)
        {
            _context.Inspection_WoItem.Add(item);
            await _context.SaveChangesAsync();
        }

        // Read (單筆)
        public async Task<Inspection_WoItem?> GetAsync(string 點檢單號, string 點檢項目)
        {
            return await _context.Inspection_WoItem
                .FindAsync(點檢單號, 點檢項目);
        }

        // Read (全部)
        public async Task<List<Inspection_WoItem>> GetAllAsync()
        {
            return await _context.Inspection_WoItem.ToListAsync();
        }

        // Update
        public async Task UpdateAsync(Inspection_WoItem item)
        {
            try
            {
                _context.Inspection_WoItem.Update(item);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {

                throw;
            }
        }
        public async Task UpsertAsync(Inspection_WoItem item)
        {
            var dbItem = await _context.Inspection_WoItem
                .FirstOrDefaultAsync(x => x.點檢單號 == item.點檢單號 && x.點檢項目 == item.點檢項目);

            try
            {
                if (dbItem == null)
                {
                    _context.Inspection_WoItem.Add(item);
                }
                else
                {
                    // 更新欄位
                    dbItem.點檢時間 = item.點檢時間;
                    dbItem.點檢內容 = item.點檢內容;
                    dbItem.錯誤項目 = item.錯誤項目;
                    dbItem.錯誤代碼 = item.錯誤代碼;
                    dbItem.分類 = item.分類;
                    dbItem.備註 = item.備註;
                    dbItem.責任單位 = item.責任單位;
                    dbItem.結果 = item.結果;
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {

                throw;
            }
        }
        // Delete
        public async Task DeleteAsync(string 點檢單號, string 點檢項目)
        {
            var item = await GetAsync(點檢單號, 點檢項目);
            if (item != null)
            {
                _context.Inspection_WoItem.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
