using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.Service
{
    public class TempratureHuLogService
    {
        private readonly MachineDBContext _db;

        public TempratureHuLogService(MachineDBContext db)
        {
            _db = db;
        }
        public async Task<temprature_Hu_log?> GetLastLogByMachineNumber(string machineNumber)
        {
            return await _db.temprature_Hu_logs
                .Where(x => x.MachineNumber == machineNumber)
                .OrderByDescending(x => x.CreateDate)
                .FirstOrDefaultAsync();
        }
        // 取得全部
        public async Task<List<temprature_Hu_log>> GetAllAsync()
        {
            return await _db.temprature_Hu_logs.AsNoTracking().ToListAsync();
        }

        // 依 Id 取得單筆
        public async Task<temprature_Hu_log?> GetByIdAsync(int id)
        {
            return await _db.temprature_Hu_logs.FindAsync(id);
        }

        // 新增或更新
        public async Task UpsertAsync(temprature_Hu_log item)
        {
            if (item.Id == 0)
            {
                _db.temprature_Hu_logs.Add(item);
            }
            else
            {
                _db.temprature_Hu_logs.Update(item);
            }
            await _db.SaveChangesAsync();
        }

        // 刪除
        public async Task DeleteAsync(int id)
        {
            var entity = await _db.temprature_Hu_logs.FindAsync(id);
            if (entity != null)
            {
                _db.temprature_Hu_logs.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }


        public async Task<List<temprature_Hu_log>> GetByMachineNumberAndDateRangeAsync(string machineNumber, DateTime start, DateTime end)
        {
            // 假設你有注入 DbContext，名稱為 _db 或 context
            return await _db.temprature_Hu_logs
                .Where(log => log.MachineNumber == machineNumber
                    && log.CreateDate >= start
                    && log.CreateDate <= end)
                .OrderBy(log => log.CreateDate)
                .ToListAsync();
        }

        // 取得每個設備的最新一筆記錄（優化效能）
        public async Task<List<temprature_Hu_log>> GetLatestLogsByAllMachinesAsync()
        {
            // 優化：查詢最近 7 天的資料，但限制總筆數以避免載入過多資料
            var recentDate = DateTime.Now.AddDays(-7);
            
            // 載入最近 7 天的記錄，按時間倒序排列，限制最多 10000 筆
            // 這樣可以確保取得最新的記錄，同時避免載入過多資料
            var recentLogs = await _db.temprature_Hu_logs
                .AsNoTracking()
                .Where(log => log.CreateDate >= recentDate)
                .OrderByDescending(log => log.CreateDate)
                .Take(10000) // 限制最多 10000 筆，避免載入過多資料
                .ToListAsync();

            // 如果最近 7 天沒有資料，嘗試查詢所有記錄（不限時間，但限制筆數）
            if (!recentLogs.Any())
            {
                recentLogs = await _db.temprature_Hu_logs
                    .AsNoTracking()
                    .OrderByDescending(log => log.CreateDate)
                    .Take(10000) // 限制最多 10000 筆
                    .ToListAsync();
            }

            // 在記憶體中分組並取得每個設備的最新記錄
            // 因為已經 OrderByDescending，所以每個組的 First 就是最新的
            return recentLogs
                .GroupBy(log => log.MachineNumber)
                .Select(g => g.First()) // 因為已經 OrderByDescending，所以 First 就是最新的
                .ToList();
        }

        // 取得最新記錄的建立時間（用於顯示數據擷取時間）
        public async Task<DateTime?> GetLatestCreateDateAsync()
        {
            return await _db.temprature_Hu_logs
                .AsNoTracking()
                .Where(log => (log.temperature ?? 0) != 0 && (log.humidity ?? 0) != 0)
                .OrderByDescending(log => log.CreateDate)
                .Select(log => log.CreateDate)
                .FirstOrDefaultAsync();
        }

        // 取得最近記錄的原始列表（不分組，用於過濾全 0 數據）
        public async Task<List<temprature_Hu_log>> GetRecentLogsRawAsync(int days = 7, int maxRecords = 10000)
        {
            var recentDate = DateTime.Now.AddDays(-days);
            
            // 載入最近 N 天的記錄，按時間倒序排列，限制最多 maxRecords 筆
            var recentLogs = await _db.temprature_Hu_logs
                .AsNoTracking()
                .Where(log => log.CreateDate >= recentDate)
                .OrderByDescending(log => log.CreateDate)
                .Take(maxRecords)
                .ToListAsync();

            // 如果最近 N 天沒有資料，嘗試查詢所有記錄（不限時間，但限制筆數）
            if (!recentLogs.Any())
            {
                recentLogs = await _db.temprature_Hu_logs
                    .AsNoTracking()
                    .OrderByDescending(log => log.CreateDate)
                    .Take(maxRecords)
                    .ToListAsync();
            }

            return recentLogs;
        }
    }
}
