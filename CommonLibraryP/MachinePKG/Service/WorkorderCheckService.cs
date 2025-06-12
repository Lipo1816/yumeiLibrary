using CommonLibraryP.API;
using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.EntityFrameworkCore;


namespace CommonLibraryP.MachinePKG.Service
{
    public class WorkorderCheckService
    {
        private readonly MachineDBContext _context;

        public WorkorderCheckService(MachineDBContext context)
        {
            _context = context;
        }

        public async Task<List<WorkorderCheck>> GetAllAsync()
        {
            return await _context.WorkorderChecks.ToListAsync();
        }

        public async Task<WorkorderCheck?> GetByIdAsync(string id)
        {
            return await _context.WorkorderChecks.FindAsync(id);
        }

        public async Task<(bool IsSuccess, string Msg)> UpsertAsync(WorkorderCheck entity)
        {
            var exist = await _context.WorkorderChecks.FindAsync(entity.工單號);
            if (exist == null)
                _context.WorkorderChecks.Add(entity);
            else
                _context.Entry(exist).CurrentValues.SetValues(entity);

            await _context.SaveChangesAsync();
            return (true, "儲存成功");
        }

        public async Task<(bool IsSuccess, string Msg)> DeleteAsync(string id)
        {
            var exist = await _context.WorkorderChecks.FindAsync(id);
            if (exist == null) return (false, "找不到資料");
            _context.WorkorderChecks.Remove(exist);
            await _context.SaveChangesAsync();
            return (true, "刪除成功");
        }
        // 取得全部 WorkorderList
        public async Task<List<WorkorderList>> GetAllWorkorderListsAsync()
        {
            return await _context.WorkorderLists.ToListAsync();
        }

        // 依 Id 取得單筆 WorkorderList
        public async Task<WorkorderList?> GetWorkorderListByIdAsync(int id)
        {
            return await _context.WorkorderLists.FindAsync(id);
        }

        // 新增或更新 WorkorderList
        public async Task<(bool IsSuccess, string Msg)> UpsertWorkorderListAsync(WorkorderList entity)
        {
            var exist = await _context.WorkorderLists.FindAsync(entity.Id);
            if (exist == null)
                _context.WorkorderLists.Add(entity);
            else
                _context.Entry(exist).CurrentValues.SetValues(entity);

            await _context.SaveChangesAsync();
            return (true, "儲存成功");
        }

        // 刪除 WorkorderList
        public async Task<(bool IsSuccess, string Msg)> DeleteWorkorderListAsync(int id)
        {
            var exist = await _context.WorkorderLists.FindAsync(id);
            if (exist == null) return (false, "找不到資料");
            _context.WorkorderLists.Remove(exist);
            await _context.SaveChangesAsync();
            return (true, "刪除成功");
        }


        public async Task AddAsync(WorkorderCheck entity)
        {
            _context.WorkorderChecks.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<WorkorderCheck?> GetByInspectionNoAsync(string inspectionNo)
        {
            return await _context.WorkorderChecks
                .FirstOrDefaultAsync(x => x.點檢單號 == inspectionNo);
        }
    }
}
