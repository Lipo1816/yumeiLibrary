using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.Service
{
    public class BreakTimeScheduleService
    {
        private readonly MachineDBContext _db;

        public BreakTimeScheduleService(MachineDBContext db)
        {
            _db = db;
        }

        // Create
        public async Task AddAsync(BreakTimeSchedule entity)
        {
            await _db.BreakTimeSchedules.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        // Read (全部)
        public async Task<List<BreakTimeSchedule>> GetAllAsync()
        {
            return await _db.BreakTimeSchedules
      .GroupBy(x => new { x.LineName, x.WeekDay, x.PeriodNo })
      .Select(g => g.OrderByDescending(x => x.ModifyTime).First())
      .ToListAsync();
        }

        // Read (依主鍵)
        public async Task<BreakTimeSchedule?> GetAsync(string lineName, string weekDay, int periodNo, DateTime modifyTime)
        {
            return await _db.BreakTimeSchedules.FindAsync(lineName, weekDay, periodNo, modifyTime);
        }

        // Update
        public async Task UpdateAsync(BreakTimeSchedule entity)
        {
            _db.BreakTimeSchedules.Update(entity);
            await _db.SaveChangesAsync();
        }

        // Delete
        public async Task DeleteAsync(string lineName, string weekDay, int periodNo, DateTime modifyTime)
        {
            var entity = await GetAsync(lineName, weekDay, periodNo, modifyTime);
            if (entity != null)
            {
                _db.BreakTimeSchedules.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }

        public async Task UpsertRangeAsync(List<BreakTimeSchedule> entities)
        {
            foreach (var entity in entities)
            {
                // 用所有主鍵查詢（包含 ModifyTime）
                var old = await _db.BreakTimeSchedules
                    .FirstOrDefaultAsync(x =>
                        x.LineName == entity.LineName &&
                        x.WeekDay == entity.WeekDay &&
                        x.PeriodNo == entity.PeriodNo );

                if (old != null)
                {
                    // 比對三個欄位是否有異動
                    bool isChanged =
                        old.StartTime != entity.StartTime ||
                        old.EndTime != entity.EndTime ||
                        old.IsEnable != entity.IsEnable;

                    if (isChanged)
                    {
                        // 不能直接改 ModifyTime（主鍵），只能刪除舊資料再新增
                        _db.BreakTimeSchedules.Remove(old);
                        entity.ModifyTime = DateTime.Now;
                        _db.BreakTimeSchedules.Add(entity);
                    }
                    // 沒異動就什麼都不做
                }
                else
                {
                    // 新增
                    entity.ModifyTime = DateTime.Now;
                    _db.BreakTimeSchedules.Add(entity);
                }
            }

            await _db.SaveChangesAsync();
        }


    }
}
