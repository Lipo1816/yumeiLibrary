using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.EntityFrameworkCore;

namespace CommonLibraryP.MachinePKG.Service
{
    public class InspecWOListService
    {
        private readonly MachineDBContext _context;

        public InspecWOListService(MachineDBContext context)
        {
            _context = context;
        }

        // 取得全部
        public async Task<List<InspecWOList>> GetAllAsync()
        {
            return await _context.InspecWOLists.ToListAsync();
        }

        // 依ID取得
        public async Task<InspecWOList?> GetByIdAsync(int id)
        {
            return await _context.InspecWOLists.FindAsync(id);
        }

        // 新增或更新
        public async Task UpsertAsync(InspecWOList entity)
        {
            if (entity.ID == 0)
                _context.InspecWOLists.Add(entity);
            else
            {
                var exist = await _context.InspecWOLists.FindAsync(entity.ID);
                if (exist != null)
                    _context.Entry(exist).CurrentValues.SetValues(entity);
            }
            await _context.SaveChangesAsync();
        }

        // 刪除
        public async Task<bool> DeleteAsync(int id)
        {
            var exist = await _context.InspecWOLists.FindAsync(id);
            if (exist == null) return false;
            _context.InspecWOLists.Remove(exist);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task AddAsync(InspecWOList entity)
        {
            _context.InspecWOLists.Add(entity);
            await _context.SaveChangesAsync();
        }

        // 你也可以加上 AddRangeAsync 批次新增
        public async Task AddRangeAsync(IEnumerable<InspecWOList> entities)
        {
            _context.InspecWOLists.AddRange(entities);
            await _context.SaveChangesAsync();
        }
    }
}
