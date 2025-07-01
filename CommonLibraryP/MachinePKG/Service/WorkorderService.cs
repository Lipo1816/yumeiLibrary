using CommonLibraryP.API;
using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.EntityFrameworkCore;

namespace CommonLibraryP.MachinePKG.Service
{
    public class WorkorderService
    {
        private readonly MachineDBContext _db;

        public WorkorderService(MachineDBContext db)
        {
            _db = db;
        }

        public async Task<List<Workorder>> GetAllWorkorders()
        {
            return await _db.Workorders.AsNoTracking().ToListAsync();
        }
        public async Task UpdateAsync(Workorder entity)
        {
            _db.Workorders.Update(entity);
            await _db.SaveChangesAsync();
        }
        public async Task<RequestResult> UpsertWorkorder(Workorder w)
        {
            var exist = await _db.Workorders.FindAsync(w.工單號);
            if (exist == null)
            {
                _db.Workorders.Add(w);
            }
            else
            {
                _db.Entry(exist).CurrentValues.SetValues(w);
            }
            await _db.SaveChangesAsync();
            // return new RequestResult { IsSuccess = true, Msg = "儲存成功" };
            return new(2, $"Upsert Workorder {w.工單號} success");
        }
        // 依工單查詢 Workorder
        public Task<Workorder?> GetWorkorderByOrderNoAsync(string orderNo)
        {
            return _db.Workorders.FirstOrDefaultAsync(x => x.工單號 == orderNo);
        }
        public async Task<Workorder?> GetByWorkOrderNoAsync(string workOrderNo)
        {
            return await _db.Workorders
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.工單號 == workOrderNo);
        }

        public async Task<RequestResult> DeleteWorkorder(string id)
        {
            var exist = await _db.Workorders.FindAsync(id);
            if (exist != null)
            {
                _db.Workorders.Remove(exist);
                await _db.SaveChangesAsync();
                return new(2, $"Delete Workorder {id} success");
            }
           // return new RequestResult { IsSuccess = false, Msg = "找不到資料" };
            return new(4, $"Delete Workorder {id} not found");
        }

        public async Task<Workorder?> GetByIdAsync(string workorderNo)
        {
            return await _db.Workorders.FindAsync(workorderNo);
        }


        public async Task AddAsync(Workorder workorder)
        {
            if (workorder == null)
                throw new ArgumentNullException(nameof(workorder));

            // 檢查主鍵是否已存在，避免重複新增
            var exists = await _db.Workorders.AnyAsync(x => x.工單號 == workorder.工單號);
            if (exists)
                throw new InvalidOperationException($"工單號 {workorder.工單號} 已存在，無法重複新增。");

            _db.Workorders.Add(workorder);
            await _db.SaveChangesAsync();
        }
    }
}