using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.EntityFrameworkCore;

namespace CommonLibraryP.MachinePKG.Service
{
    public class InspectionService
    {
        private readonly MachineDBContext _context;

        public InspectionService(MachineDBContext context)
        {
            _context = context;
        }

        public async Task<List<Inspection>> GetAllAsync()
        {
            return await _context.Inspections.ToListAsync();
        }

        public async Task<Inspection?> GetByIdAsync(string machineNo, string item)
        {
            return await _context.Inspections.FindAsync(machineNo, item);
        }

        public async Task<(bool IsSuccess, string Msg)> UpsertAsync(Inspection entity)
        {
            var exist = await _context.Inspections.FindAsync(entity.機台編號, entity.項目);
            if (exist == null)
                _context.Inspections.Add(entity);
            else
                _context.Entry(exist).CurrentValues.SetValues(entity);

            await _context.SaveChangesAsync();
            return (true, "儲存成功");
        }

        public async Task<(bool IsSuccess, string Msg)> DeleteAsync(string machineNo, string item)
        {
            var exist = await _context.Inspections.FindAsync(machineNo, item);
            if (exist == null) return (false, "找不到資料");
            _context.Inspections.Remove(exist);
            await _context.SaveChangesAsync();
            return (true, "刪除成功");
        }
        public async Task<InspectionReportTime?> GetReportTimeAsync(string type)
        {
            return await _context.InspectionReportTimes.FirstOrDefaultAsync(x => x.Type == type);
        }

        public async Task UpsertReportTimeAsync(InspectionReportTime time)
        {
            var entity = await _context.InspectionReportTimes.FirstOrDefaultAsync(x => x.Type == time.Type);
            if (entity == null)
            {
                _context.InspectionReportTimes.Add(time);
            }
            else
            {
                entity.DailyTime = time.DailyTime;
                entity.WeeklyDay = time.WeeklyDay;
                entity.WeeklyTime = time.WeeklyTime;
                entity.MonthlyDay = time.MonthlyDay;
                entity.MonthlyTime = time.MonthlyTime;
            }
            await _context.SaveChangesAsync();
        }

        public async Task AddInspectionRecordAsync(InspectionRecord record)
        {
            _context.InspectionRecords.Add(record);
            await _context.SaveChangesAsync();
        }

        public async Task<List<InspectionRecord>> GetInspectionRecordsAsync()
        {
            return await _context.InspectionRecords.ToListAsync();
        }
        // 取得全部 InspectionList
public async Task<List<InspectionList>> GetAllInspectionListsAsync()
        {
            return await _context.InspectionLists.ToListAsync();
        }

        // 依 Id 取得單筆 InspectionList
        public async Task<InspectionList?> GetInspectionListByIdAsync(int id)
        {
            return await _context.InspectionLists.FindAsync(id);
        }

        // 新增或更新 InspectionList
        public async Task<(bool IsSuccess, string Msg)> UpsertInspectionListAsync(InspectionList entity)
        {
            var exist = await _context.InspectionLists.FindAsync(entity.Id);
            if (exist == null)
                _context.InspectionLists.Add(entity);
            else
                _context.Entry(exist).CurrentValues.SetValues(entity);

            await _context.SaveChangesAsync();
            return (true, "儲存成功");
        }

        // 刪除 InspectionList
        public async Task<(bool IsSuccess, string Msg)> DeleteInspectionListAsync(int id)
        {
            var exist = await _context.InspectionLists.FindAsync(id);
            if (exist == null) return (false, "找不到資料");
            _context.InspectionLists.Remove(exist);
            await _context.SaveChangesAsync();
            return (true, "刪除成功");
        }
        public async Task UpdateInspectionRecordsAsync(IEnumerable<InspectionRecord> records)
        {
            foreach (var record in records)
            {
                var entity = await _context.InspectionRecords.FindAsync(record.Id);
                if (entity != null)
                {
                    // 更新欄位（可依實際需求調整）
                    entity.紀錄值 = record.紀錄值;
                    entity.檢查人 = record.檢查人;
                    entity.完成時間 = record.完成時間;
                    entity.表單狀態 = record.表單狀態;
                    // 其他欄位如需更新可一併處理
                }
            }
            await _context.SaveChangesAsync();
        }
        public async Task UpdateInspectionListStatusAsync(string 單號, string? 檢查人 = null)
        {
            var list = _context.InspectionLists.FirstOrDefault(l => l.單號 == 單號);
            if (list != null)
            {
                // 狀態轉換
                if (list.表單狀態 == InspectionFormStatus.UndoCheck)
                    list.表單狀態 = InspectionFormStatus.CheckDone;
                else if (list.表單狀態 == InspectionFormStatus.CheckDone)
                    list.表單狀態 = InspectionFormStatus.ModifyCheck;

                // 設定檢查人
                if (!string.IsNullOrEmpty(檢查人))
                    list.檢查人 = 檢查人;

                await _context.SaveChangesAsync();
            }
        }
    }
}