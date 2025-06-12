using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.EntityFrameworkCore;

namespace CommonLibraryP.MachinePKG.Service
{
    public class ReportWorkOrderService
    {
        private readonly MachineDBContext _context;

        public ReportWorkOrderService(MachineDBContext context)
        {
            _context = context;
        }

        // 取得全部
        public async Task<List<ReportWorkOrder>> GetAllAsync()
        {
            return await _context.ReportWorkOrders.ToListAsync();
        }

        // 依ID取得
        public async Task<ReportWorkOrder?> GetByIdAsync(int id)
        {
            return await _context.ReportWorkOrders.FindAsync(id);
        }

        // 新增或更新
        public async Task UpsertAsync(ReportWorkOrder entity)
        {
            if (entity.ID == 0)
                _context.ReportWorkOrders.Add(entity);
            else
            {
                var exist = await _context.ReportWorkOrders.FindAsync(entity.ID);
                if (exist != null)
                    _context.Entry(exist).CurrentValues.SetValues(entity);
            }
            await _context.SaveChangesAsync();
        }

        // 刪除
        public async Task<bool> DeleteAsync(int id)
        {
            var exist = await _context.ReportWorkOrders.FindAsync(id);
            if (exist == null) return false;
            _context.ReportWorkOrders.Remove(exist);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
