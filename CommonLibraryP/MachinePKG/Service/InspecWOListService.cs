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

        public async Task UpdateAsync(InspecWOList entity)
        {
            // 取得現有資料
            var dbEntity = await _context.InspecWOLists
                .FirstOrDefaultAsync(x => x.ID == entity.ID);

            if (dbEntity == null)
                return;

            // 更新所有欄位
            dbEntity.工單 = entity.工單;
            dbEntity.點檢單號 = entity.點檢單號;
            dbEntity.狀態 = entity.狀態;
            dbEntity.報工時間 = entity.報工時間;
            dbEntity.產生時間 = entity.產生時間;
            dbEntity.報工人員 = entity.報工人員;
            dbEntity.Type = entity.Type;
            dbEntity.錯誤項目1 = entity.錯誤項目1;
            dbEntity.錯誤項目2 = entity.錯誤項目2;
            dbEntity.錯誤項目3 = entity.錯誤項目3;
            dbEntity.錯誤項目4 = entity.錯誤項目4;
            dbEntity.錯誤描述 = entity.錯誤描述;
            dbEntity.result = entity.result;

            await _context.SaveChangesAsync();
        }


        // 取得全部
        public async Task<List<InspecWOList>> GetAllAsync()
        {
            try
            {
                return await _context.InspecWOLists.ToListAsync();
            }
            catch (Exception ex)
            {
                // 可以打斷點或 log ex.Message
                throw;
            }
        }
        // 依點檢單號查詢 InspecWOList
        public Task<InspecWOList?> GetInspecWOListByInspectionNoAsync(string inspectionNo)
        {
            return _context.InspecWOLists.FirstOrDefaultAsync(x => x.點檢單號 == inspectionNo);
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
        public async Task<List<InspecWOList>> GetByWorkOrderAsync(string workOrderNo)
        {
            if (string.IsNullOrWhiteSpace(workOrderNo))
                return new List<InspecWOList>();

            return await _context.InspecWOLists
                .Where(x => x.工單 == workOrderNo)
                .ToListAsync();
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
