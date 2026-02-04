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
        private readonly IDbContextFactory<MachineDBContext> _dbFactory;

        public TempratureHuLogService(IDbContextFactory<MachineDBContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }
        public async Task<temprature_Hu_log?> GetLastLogByMachineNumber(string machineNumber)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            return await db.temprature_Hu_logs
                .Where(x => x.MachineNumber == machineNumber)
                .OrderByDescending(x => x.CreateDate)
                .FirstOrDefaultAsync();
        }
        // 取得全部
        public async Task<List<temprature_Hu_log>> GetAllAsync()
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            return await db.temprature_Hu_logs.AsNoTracking().ToListAsync();
        }

        // 依 Id 取得單筆
        public async Task<temprature_Hu_log?> GetByIdAsync(int id)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            return await db.temprature_Hu_logs.FindAsync(id);
        }

        // 新增或更新
        public async Task UpsertAsync(temprature_Hu_log item)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            if (item.Id == 0)
            {
                db.temprature_Hu_logs.Add(item);
            }
            else
            {
                db.temprature_Hu_logs.Update(item);
            }
            await db.SaveChangesAsync();
        }

        // 刪除
        public async Task DeleteAsync(int id)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            var entity = await db.temprature_Hu_logs.FindAsync(id);
            if (entity != null)
            {
                db.temprature_Hu_logs.Remove(entity);
                await db.SaveChangesAsync();
            }
        }


        public async Task<List<temprature_Hu_log>> GetByMachineNumberAndDateRangeAsync(string machineNumber, DateTime start, DateTime end)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            return await db.temprature_Hu_logs
                .Where(log => log.MachineNumber == machineNumber
                    && log.CreateDate >= start
                    && log.CreateDate <= end)
                .OrderBy(log => log.CreateDate)
                .ToListAsync();
        }

        /// <summary>
        /// 高效取得每個設備的最新一筆有效日誌（溫度或濕度不為0）
        /// 使用資料庫層面的查詢，避免載入所有資料到記憶體
        /// </summary>
        public async Task<Dictionary<string, temprature_Hu_log>> GetLatestValidLogsByMachineNumbersAsync(IEnumerable<string> machineNumbers)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            //////// 轉成清單以便多次使用
            var machineNumberList = machineNumbers.ToList();
            if (!machineNumberList.Any())
                return new Dictionary<string, temprature_Hu_log>();

            // 先找出每個設備的最新 CreateDate
            var latestDates = await db.temprature_Hu_logs
                .Where(log => machineNumberList.Contains(log.MachineNumber) 
                    && log.temperature.HasValue && log.temperature.Value != 0
                    && log.humidity.HasValue && log.humidity.Value != 0)
                .GroupBy(log => log.MachineNumber)
                .Select(g => new { MachineNumber = g.Key, MaxDate = g.Max(x => x.CreateDate) })
                .ToListAsync();

            if (!latestDates.Any())
                return new Dictionary<string, temprature_Hu_log>();

            // 根據最新日期取得對應的日誌
            var result = new Dictionary<string, temprature_Hu_log>();
            foreach (var item in latestDates)
            {
                var log = await db.temprature_Hu_logs
                    .Where(x => x.MachineNumber == item.MachineNumber 
                        && x.CreateDate == item.MaxDate
                        && x.temperature.HasValue && x.temperature.Value != 0
                        && x.humidity.HasValue && x.humidity.Value != 0)
                    .OrderByDescending(x => x.Id) // 如果有多筆相同時間，取最新的 ID
                    .FirstOrDefaultAsync();
                
                if (log != null)
                    result[item.MachineNumber] = log;
            }
            ///// 上面的方法會對每個設備執行一次查詢，對於大量設備可能效率不佳，可以改為一次查詢取得所有最新日誌，再在記憶體中過濾
            return result;
        }

        /// <summary>
        /// 取得所有設備的最新一筆有效日誌（更高效的方法，使用單一查詢）
        /// </summary>
        public async Task<Dictionary<string, temprature_Hu_log>> GetLatestValidLogsForAllMachinesAsync()
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            
            // 使用子查詢找出每個設備的最新日誌 ID
            var latestLogIds = await db.temprature_Hu_logs
                .Where(log => log.temperature.HasValue && log.temperature.Value != 0
                    && log.humidity.HasValue && log.humidity.Value != 0)
                .GroupBy(log => log.MachineNumber)
                .Select(g => new 
                { 
                    MachineNumber = g.Key, 
                    MaxId = g.Max(x => x.Id) 
                })
                .ToListAsync();

            if (!latestLogIds.Any())
                return new Dictionary<string, temprature_Hu_log>();

            var ids = latestLogIds.Select(x => x.MaxId).ToList();
            var logs = await db.temprature_Hu_logs
                .Where(log => ids.Contains(log.Id))
                .AsNoTracking()
                .ToListAsync();

            return logs.ToDictionary(log => log.MachineNumber, log => log);
        }

        /// <summary>
        /// 取得最新的有效日誌建立時間（用於顯示數據擷取時間）
        /// </summary>
        public async Task<DateTime?> GetLatestValidLogDateAsync()
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            return await db.temprature_Hu_logs
                .Where(log => log.temperature.HasValue && log.temperature.Value != 0
                    && log.humidity.HasValue && log.humidity.Value != 0)
                .OrderByDescending(log => log.CreateDate)
                .Select(log => log.CreateDate)
                .FirstOrDefaultAsync();
        }
    }
}
