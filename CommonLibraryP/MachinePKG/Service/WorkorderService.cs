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
    }
}