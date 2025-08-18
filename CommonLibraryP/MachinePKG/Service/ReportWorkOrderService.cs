using CommonLibraryP.MachinePKG.EFModel;
using DevExpress.ReportServer.ServiceModel.DataContracts;
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
        public async Task<ReportWorkOrder?> GetLatestByWorkorderNoAsync(string workorderNo)
        {
            if (string.IsNullOrWhiteSpace(workorderNo))
            {
                return null;
            }

            // 依工單號查詢最新的 ReportWO，假設以 CreateTime 判斷最新記錄
            return await _context.ReportWorkOrders
                .Where(r => r.工單 == workorderNo)
                .OrderByDescending(r => r.報工時間)
                .FirstOrDefaultAsync();
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

        public async Task AddAsync(ReportWorkOrder entity)
        {
            try
            {
                _context.ReportWorkOrders.Add(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<ReportWorkOrder?> GetByWorkOrderNoClosestToNowAsync(string workOrderNo)
        {
            var now = DateTime.Now;

            var d = _context.ReportWorkOrders.Where(x => x.工單 == workOrderNo);

            return await _context.ReportWorkOrders
                .Where(x => x.工單 == workOrderNo)
                .OrderBy(x => Math.Abs(EF.Functions.DateDiffSecond(x.報工時間, now)))
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(ReportWorkOrder entity)
        {
            _context.ReportWorkOrders.Update(entity);
            await _context.SaveChangesAsync();
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
