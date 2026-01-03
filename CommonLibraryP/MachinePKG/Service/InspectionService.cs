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
                entity.QuarterMonth = time.QuarterMonth;
                entity.QuarterDay = time.QuarterDay;
                entity.QuarterTime = time.QuarterTime;
                entity.YearMonth = time.YearMonth;
                entity.YearDay = time.YearDay;
                entity.YearTime = time.YearTime;
                entity.DailyEnable = time.DailyEnable;
                entity.WeeklyEnable = time.WeeklyEnable;
                entity.MonthlyEnable = time.MonthlyEnable;
                entity.QuarterEnable = time.QuarterEnable;
                entity.YearEnable = time.YearEnable;



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
            try
            {
                return await _context.InspectionRecords.ToListAsync();
            }
            catch (Exception e)
            {

                throw;
            }
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
                    entity.完成時間 = DateTime.Now; //record.完成時間;
                    entity.表單狀態 = record.表單狀態;
                    entity.檢查 = record.檢查;
                    entity.標準 = record.標準;
                    entity.方式 = record.方式;
                    entity.檢查點位 = record.檢查點位;
                    entity.結果 = record.結果;
                    entity.維修期限 = record.維修期限;
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

                list.完成時間 = DateTime.Now;
                // 設定檢查人
                if (!string.IsNullOrEmpty(檢查人))
                    list.檢查人 = 檢查人;

                await _context.SaveChangesAsync();
            }
        }
        public async Task<InspectionList?> GetInspectionListAsync(string orderNo, string machineName)
        {
            return await _context.InspectionLists
                .FirstOrDefaultAsync(x => x.單號 == orderNo && x.機台名稱 == machineName);
        }

        public async Task UpdateInspectionListAsync(InspectionList inspectionList)
        {
            if (inspectionList == null)
                throw new ArgumentNullException(nameof(inspectionList));

            _context.InspectionLists.Update(inspectionList);
            await _context.SaveChangesAsync();
        }

        // 清空所有「未檢驗」狀態的點檢表單資料
        // 針對「點檢判定」（結果）、「維修確認」（維修完成）、「綜合判定」欄位（檢查結果）進行清空
        public async Task<(bool IsSuccess, string Msg, int ClearedCount)> ClearUncheckedInspectionFormsAsync()
        {
            try
            {
                int clearedCount = 0;

                // 1. 清空 InspectionList 中「未檢驗」狀態的「檢查結果」欄位（綜合判定）
                var uncheckedLists = await _context.InspectionLists
                    .Where(x => string.IsNullOrEmpty(x.檢查結果) || x.檢查結果 == "未檢驗")
                    .ToListAsync();

                foreach (var list in uncheckedLists)
                {
                    list.檢查結果 = null;
                    clearedCount++;
                }

                // 2. 清空 InspectionRecord 中「未檢驗」狀態的「結果」欄位（點檢判定）和「維修完成」欄位（維修確認）
                // 先取得所有「未檢驗」狀態的 InspectionList 的單號
                var uncheckedOrderNos = uncheckedLists.Select(x => x.單號).ToList();

                if (uncheckedOrderNos.Any())
                {
                    var uncheckedRecords = await _context.InspectionRecords
                        .Where(x => uncheckedOrderNos.Contains(x.檢查單號))
                        .ToListAsync();

                    foreach (var record in uncheckedRecords)
                    {
                        // 清空「點檢判定」（結果）
                        record.結果 = null;
                        // 清空「維修確認」（維修完成）
                        record.維修完成 = null;
                        // 同時清空相關欄位
                        record.維修完成日 = null;
                        record.未完成類別 = null;
                        record.未完成說明 = null;
                        record.維修期限 = null;
                    }
                }

                await _context.SaveChangesAsync();

                return (true, $"成功清空 {clearedCount} 個「未檢驗」狀態的點檢表單", clearedCount);
            }
            catch (Exception ex)
            {
                return (false, $"清空資料時發生錯誤: {ex.Message}", 0);
            }
        }

        // 補產生今日點檢表（手動觸發）
        public async Task<(bool IsSuccess, string Msg, int GeneratedCount)> GenerateTodayInspectionRecordsAsync(string type)
        {
            try
            {
                var now = DateTime.Now;
                int generatedCount = 0;

                // 根據類型取得對應的點檢項目
                string frequency = type switch
                {
                    "Daily" => "日",
                    "Weekly" => "周",
                    "Monthly" => "月",
                    "Quarterly" => "季",
                    "Yearly" => "年",
                    _ => throw new ArgumentException($"不支援的類型: {type}")
                };

                var inspections = await _context.Inspections
                    .Where(x => x.頻率 == frequency)
                    .ToListAsync();

                if (!inspections.Any())
                {
                    return (false, $"沒有找到頻率為「{frequency}」的點檢項目", 0);
                }

                var groups = inspections.GroupBy(x => x.機台編號);
                string checknumber = type switch
                {
                    "Daily" => "DI_" + now.ToString("yyyyMMddHH") + "_",
                    "Weekly" => "WI_" + now.ToString("yyyyMMddHH") + "_",
                    "Monthly" => "MI_" + now.ToString("yyyyMMddHH") + "_",
                    "Quarterly" => "QI_" + now.ToString("yyyyMMddHH") + "_",
                    "Yearly" => "YI_" + now.ToString("yyyyMMddHH") + "_",
                    _ => ""
                };

                foreach (var group in groups)
                {
                    // 檢查該機台今天是否已經產生過（根據類型判斷）
                    bool alreadyExists = false;
                    if (type == "Daily")
                    {
                        alreadyExists = await _context.InspectionLists.AnyAsync(l =>
                            l.機台編號 == group.Key &&
                            l.產生時間.Date == now.Date &&
                            l.TYPE == type);
                    }
                    else if (type == "Weekly")
                    {
                        var cal = System.Globalization.CultureInfo.CurrentCulture.Calendar;
                        var weekRule = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
                        var thisWeek = cal.GetWeekOfYear(now, weekRule, DayOfWeek.Monday);
                        // 先載入到記憶體，避免 EF Core 無法翻譯 GetWeekOfYear
                        var existingLists = await _context.InspectionLists
                            .Where(l => l.機台編號 == group.Key && 
                                       l.產生時間.Year == now.Year && 
                                       l.TYPE == type)
                            .ToListAsync();
                        alreadyExists = existingLists.Any(l =>
                            cal.GetWeekOfYear(l.產生時間, weekRule, DayOfWeek.Monday) == thisWeek);
                    }
                    else if (type == "Monthly")
                    {
                        alreadyExists = await _context.InspectionLists.AnyAsync(l =>
                            l.機台編號 == group.Key &&
                            l.產生時間.Year == now.Year &&
                            l.產生時間.Month == now.Month &&
                            l.TYPE == type);
                    }
                    else if (type == "Quarterly" || type == "Yearly")
                    {
                        alreadyExists = await _context.InspectionLists.AnyAsync(l =>
                            l.機台編號 == group.Key &&
                            l.產生時間.Year == now.Year &&
                            l.產生時間.Month == now.Month &&
                            l.TYPE == type);
                    }

                    if (alreadyExists) continue; // 該機台已經產生過，跳過

                    // 產生 InspectionRecord
                    foreach (var item in group)
                    {
                        bool exists = false;
                        if (type == "Daily")
                        {
                            exists = await _context.InspectionRecords.AnyAsync(r =>
                                r.機台編號 == item.機台編號 &&
                                r.項目 == item.項目 &&
                                r.產生時間.Date == now.Date);
                        }
                        else if (type == "Weekly")
                        {
                            var cal = System.Globalization.CultureInfo.CurrentCulture.Calendar;
                            var weekRule = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
                            var thisWeek = cal.GetWeekOfYear(now, weekRule, DayOfWeek.Monday);
                            // 先載入到記憶體，避免 EF Core 無法翻譯 GetWeekOfYear
                            var existingRecords = await _context.InspectionRecords
                                .Where(r => r.機台編號 == item.機台編號 &&
                                           r.項目 == item.項目 &&
                                           r.產生時間.Year == now.Year)
                                .ToListAsync();
                            exists = existingRecords.Any(r =>
                                cal.GetWeekOfYear(r.產生時間, weekRule, DayOfWeek.Monday) == thisWeek);
                        }
                        else if (type == "Monthly" || type == "Quarterly" || type == "Yearly")
                        {
                            exists = await _context.InspectionRecords.AnyAsync(r =>
                                r.機台編號 == item.機台編號 &&
                                r.項目 == item.項目 &&
                                r.檢查單號 == checknumber + item.機台編號 &&
                                r.產生時間.Year == now.Year &&
                                r.產生時間.Month == now.Month);
                        }

                        if (exists) continue;

                        var record = new InspectionRecord
                        {
                            機台編號 = item.機台編號,
                            機台名稱 = item.機台名稱,
                            項目 = item.項目,
                            產生時間 = now,
                            檢查人 = "",
                            表單狀態 = InspectionFormStatus.UndoCheck,
                            檢查單號 = checknumber + item.機台編號,
                            檢查 = item.檢查,
                            標準 = item.標準,
                            方式 = item.方式,
                            檢查點位 = item.點檢位置,
                            結果 = null
                        };
                        _context.InspectionRecords.Add(record);
                    }

                    // 產生 InspectionList（每台機台一筆）
                    var inspectionList = new InspectionList
                    {
                        機台編號 = group.Key,
                        機台名稱 = group.First().機台名稱 ?? "",
                        產生時間 = now,
                        TYPE = type,
                        表單狀態 = InspectionFormStatus.UndoCheck,
                        檢查人 = "",
                        單號 = checknumber + group.Key
                    };
                    _context.InspectionLists.Add(inspectionList);
                    generatedCount++;
                }

                await _context.SaveChangesAsync();
                
                if (generatedCount > 0)
                {
                    return (true, $"成功補產生 {type} 點檢表，共 {generatedCount} 台機台", generatedCount);
                }
                else
                {
                    return (true, $"所有 {type} 點檢表已存在，無需補產生", 0);
                }
            }
            catch (Exception ex)
            {
                return (false, $"補產生點檢表時發生錯誤: {ex.Message}", 0);
            }
        }
    }
}