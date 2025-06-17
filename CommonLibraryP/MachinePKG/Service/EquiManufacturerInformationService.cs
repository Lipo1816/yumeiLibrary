using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.Service
{
    public class EquiManufacturerInformationService
    {
        private readonly MachineDBContext _context;

        public EquiManufacturerInformationService(MachineDBContext context)
        {
            _context = context;
        }

        // 取得全部
        public async Task<List<EquiManufacturer_Information>> GetAllAsync()
        {
            return await _context.EquiManufacturer_Informations.ToListAsync();
        }

        // 依 Id 取得單筆
        public async Task<EquiManufacturer_Information?> GetByIdAsync(int id)
        {
            return await _context.EquiManufacturer_Informations.FindAsync(id);
        }

        // 新增
        public async Task AddAsync(EquiManufacturer_Information info)
        {
            _context.EquiManufacturer_Informations.Add(info);
            await _context.SaveChangesAsync();
        }

        // 更新
        public async Task UpdateAsync(EquiManufacturer_Information info)
        {
            _context.EquiManufacturer_Informations.Update(info);
            await _context.SaveChangesAsync();
        }

        // 刪除
        public async Task DeleteAsync(int id)
        {
            var entity = await _context.EquiManufacturer_Informations.FindAsync(id);
            if (entity != null)
            {
                _context.EquiManufacturer_Informations.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<EquiManufacturer_Information> GetByMachineNameAsync(string machineName)
        {
            return await _context.EquiManufacturer_Informations
                .Where(x => x.機台名稱 == machineName)
                .FirstOrDefaultAsync();
        }

    }
}
