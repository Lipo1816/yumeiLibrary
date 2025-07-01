using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            return await _context.Inspection_WoItem
                .Where(x => x.點檢單號 == inspectionNo)
                .ToListAsync();
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
            _context.Inspection_WoItem.Update(item);
            await _context.SaveChangesAsync();
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
