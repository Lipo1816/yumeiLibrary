using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.Service
{
    public class InspectionMonitorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InspectionMonitorService> _logger;

        public InspectionMonitorService(IServiceProvider serviceProvider, ILogger<InspectionMonitorService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }



        private async Task CheckAndGenerateDailyInspectionRecords()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();

                // 取得日檢設定
                var dailySetting = db.InspectionReportTimes.FirstOrDefault(x => x.Type == "Daily");
                if (dailySetting == null)
                {
                    _logger.LogDebug("日檢設定不存在");
                    return;
                }
                
                if (dailySetting.DailyEnable != true)
                {
                    _logger.LogDebug("日檢功能已停用");
                    return;
                }
                
                if (dailySetting.DailyTime == null)
                {
                    _logger.LogWarning("日檢時間未設定");
                    return;
                }

                var now = DateTime.Now;
                var timeDiff = Math.Abs((now.TimeOfDay - dailySetting.DailyTime.Value).TotalMinutes);
                
                // 判斷是否到達日檢時間（允許誤差3分鐘）
                // 先檢查今天是否已經產生過點檢表，避免重複產生
                bool todayAlreadyGenerated = db.InspectionLists.Any(l =>
                    l.產生時間.Date == now.Date &&
                    l.TYPE == "Daily"
                );
                
                if (todayAlreadyGenerated)
                {
                    _logger.LogDebug($"今天 ({now:yyyy-MM-dd}) 已產生過日檢表，跳過");
                    return;
                }

                if (now.TimeOfDay.Hours == dailySetting.DailyTime.Value.Hours && timeDiff < 3)
            {
                var inspections = db.Inspections.Where(x => x.頻率 == "日").ToList();
                var groups = inspections.GroupBy(x => x.機台編號);

                string checknumber = "DI_" + now.ToString("yyyyMMddHH") + "_";

                foreach (var group in groups)
                {
                    // 檢查該機台今天是否已經產生過
                    bool machineTodayExists = db.InspectionLists.Any(l =>
                        l.機台編號 == group.Key &&
                        l.產生時間.Date == now.Date &&
                        l.TYPE == "Daily"
                    );
                    if (machineTodayExists) continue; // 該機台今天已經產生過，跳過

                    foreach (var item in group)
                    {
                        bool exists = db.InspectionRecords.Any(r =>
                            r.機台編號 == item.機台編號 &&
                            r.項目 == item.項目 &&
                            r.產生時間.Date == now.Date
                        );
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
                                // 新增欄位
                            檢查 = item.檢查,   // 若 item 有此屬性，否則可設 null
                            標準 = item.標準,
                            方式 = item.方式,
                            檢查點位 = item.點檢位置,
                            結果 = null         // 預設 null，或依邏輯給值

                        };
                        db.InspectionRecords.Add(record);
                    }

                    // 產生 InspectionList（每台機台一筆，避免重複）
                    bool listExists = db.InspectionLists.Any(l =>
                        l.機台編號 == group.Key &&
                        l.產生時間.Date == now.Date &&
                        l.TYPE == "Daily"
                    );
                    if (!listExists)
                    {
                        var inspectionList = new InspectionList
                        {
                            機台編號 = group.Key,
                            機台名稱 = group.First().機台名稱 ?? "",
                            產生時間 = now,
                            TYPE = "Daily",
                            表單狀態 = InspectionFormStatus.UndoCheck,
                            檢查人 = "",
                            單號 = checknumber + group.Key
                        };
                        db.InspectionLists.Add(inspectionList);
                    }
                }
                await db.SaveChangesAsync();
                _logger.LogInformation($"成功產生日檢表，時間: {now:yyyy-MM-dd HH:mm:ss}，共 {groups.Count()} 台機台");
            }
            else
            {
                _logger.LogDebug($"尚未到達日檢時間。目前時間: {now:HH:mm:ss}，設定時間: {dailySetting.DailyTime.Value:HH:mm:ss}，時間差: {timeDiff:F1} 分鐘");
            }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "產生日檢表時發生錯誤");
            }
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("點檢表自動產生服務已啟動");
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndGenerateDailyInspectionRecords();
                    await CheckAndGenerateWeeklyInspectionRecords();
                    await CheckAndGenerateMonthlyInspectionRecords();
                    await CheckAndGenerateQuarterlyInspectionRecords();
                    await CheckAndGenerateYearlyInspectionRecords();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "檢查點檢表產生時發生錯誤");
                }
                
                await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken); // 每2分鐘檢查一次
            }
        }

        private async Task CheckAndGenerateWeeklyInspectionRecords()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();

                var weeklySetting = db.InspectionReportTimes.FirstOrDefault(x => x.Type == "Weekly");
                if (weeklySetting == null)
                {
                    _logger.LogDebug("周檢設定不存在");
                    return;
                }
                
                if (weeklySetting.WeeklyEnable != true || weeklySetting.WeeklyDay == null || weeklySetting.WeeklyTime == null)
                {
                    _logger.LogDebug("周檢功能已停用或設定不完整");
                    return;
                }

            var now = DateTime.Now;
            // 判斷是否到達指定周檢時間（允許誤差3分鐘）
            // 將英文星期轉換為中文（例如：Monday -> 星期一）
            string currentDayName = GetChineseDayName(now.DayOfWeek);
            if (currentDayName == weeklySetting.WeeklyDay &&
                now.TimeOfDay.Hours == weeklySetting.WeeklyTime.Value.Hours &&
                Math.Abs(now.TimeOfDay.Minutes - weeklySetting.WeeklyTime.Value.Minutes) < 3)
            {
                var inspections = db.Inspections.Where(x => x.頻率 == "周").ToList();
                var groups = inspections.GroupBy(x => x.機台編號);

                var cal = System.Globalization.CultureInfo.CurrentCulture.Calendar;
                var weekRule = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
                var thisWeek = cal.GetWeekOfYear(now, weekRule, DayOfWeek.Monday);

                string checknumber = "WI_" + now.ToString("yyyyMMddHH") + "_";

                foreach (var group in groups)
                {
                    foreach (var item in group)
                    {
                        bool exists = db.InspectionRecords.Any(r =>
                            r.機台編號 == item.機台編號 &&
                            r.項目 == item.項目 &&
                            r.產生時間.Year == now.Year &&
                            cal.GetWeekOfYear(r.產生時間, weekRule, DayOfWeek.Monday) == thisWeek
                        );
                        if (exists) continue;

                        var record = new InspectionRecord
                        {
                            機台編號 = item.機台編號,
                            機台名稱 = item.機台名稱,
                            項目 = item.項目,
                            產生時間 = now,
                            檢查人 = "",
                            表單狀態 = InspectionFormStatus.UndoCheck,
                            檢查點位 = item.點檢位置,
                            檢查單號 = checknumber + item.機台編號,
                             結果 = null
                        };
                        db.InspectionRecords.Add(record);
                    }

                    // 產生 InspectionList（每台機台一筆，避免重複）
                    bool listExists = db.InspectionLists.Any(l =>
                        l.機台編號 == group.Key &&
                        l.產生時間.Year == now.Year &&
                        cal.GetWeekOfYear(l.產生時間, weekRule, DayOfWeek.Monday) == thisWeek &&
                        l.TYPE == "Weekly"
                    );
                    if (!listExists)
                    {
                        var inspectionList = new InspectionList
                        {
                            機台編號 = group.Key,
                            機台名稱 = group.First().機台名稱 ?? "",
                            產生時間 = now,
                            TYPE = "Weekly",
                            表單狀態 = InspectionFormStatus.UndoCheck,
                            檢查人 = "",
                            單號 = checknumber + group.Key
                        };
                        db.InspectionLists.Add(inspectionList);
                    }
                }
                await db.SaveChangesAsync();
                _logger.LogInformation($"成功產生周檢表，時間: {now:yyyy-MM-dd HH:mm:ss}，共 {groups.Count()} 台機台");
            }
            else
            {
                _logger.LogDebug($"尚未到達周檢時間。目前: {currentDayName} {now:HH:mm:ss}，設定: {weeklySetting.WeeklyDay} {weeklySetting.WeeklyTime.Value:HH:mm:ss}");
            }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "產生周檢表時發生錯誤");
            }
        }



        private async Task CheckAndGenerateMonthlyInspectionRecords()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();

                var monthlySetting = db.InspectionReportTimes.FirstOrDefault(x => x.Type == "Monthly");
                if (monthlySetting == null)
                {
                    _logger.LogDebug("月檢設定不存在");
                    return;
                }
                
                if (monthlySetting.MonthlyEnable != true || monthlySetting.MonthlyDay == null || monthlySetting.MonthlyTime == null)
                {
                    _logger.LogDebug("月檢功能已停用或設定不完整");
                    return;
                }

            var now = DateTime.Now;
            // 判斷是否到達指定月檢時間（允許誤差3分鐘）
            if (now.Day == monthlySetting.MonthlyDay &&
                now.TimeOfDay.Hours == monthlySetting.MonthlyTime.Value.Hours &&
                Math.Abs(now.TimeOfDay.Minutes - monthlySetting.MonthlyTime.Value.Minutes) < 3)
            {
                var inspections = db.Inspections.Where(x => x.頻率 == "月").ToList();
                var groups = inspections.GroupBy(x => x.機台編號);

                string checknumber = "MI_" + DateTime.Now.ToString("yyyyMMddHH") + "_";

                foreach (var group in groups)
                {
                    foreach (var item in group)
                    {
                        bool exists = db.InspectionRecords.Any(r =>
                            r.機台編號 == item.機台編號 &&
                            r.項目 == item.項目 &&
                            r.檢查單號 == checknumber+ item.機台編號 &&
                            r.產生時間.Year == now.Year &&
                            r.產生時間.Month == now.Month
                        );
                        if (exists) continue;

                        var record = new InspectionRecord
                        {
                            機台編號 = item.機台編號,
                            機台名稱 = item.機台名稱,
                            項目 = item.項目,
                            產生時間 = now,
                            檢查人 = "",
                            檢查點位 = item.點檢位置,
                            表單狀態 = InspectionFormStatus.UndoCheck,
                            檢查單號 = checknumber + item.機台編號,
                            結果 = null
                        };
                        db.InspectionRecords.Add(record);
                    }

                    // 產生 InspectionList（每台機台一筆）
                    bool listExists = db.InspectionLists.Any(l =>
                        l.機台編號 == group.Key &&
                        l.產生時間.Year == now.Year &&
                        l.產生時間.Month == now.Month &&
                        l.TYPE == "Monthly"
                    );
                    if (!listExists)
                    {
                        var inspectionList = new InspectionList
                        {
                            機台編號 = group.Key,
                            機台名稱 = group.First().機台名稱 ?? "",
                            產生時間 = now,
                            TYPE = "Monthly",
                            表單狀態 = InspectionFormStatus.UndoCheck,
                            檢查人 = "", // 可依需求填入
                            單號 = checknumber + group.Key
                        };
                        db.InspectionLists.Add(inspectionList);
                    }



                }
                await db.SaveChangesAsync();
                _logger.LogInformation($"成功產生月檢表，時間: {now:yyyy-MM-dd HH:mm:ss}，共 {groups.Count()} 台機台");
            }
            else
            {
                _logger.LogDebug($"尚未到達月檢時間。目前: {now.Day}日 {now:HH:mm:ss}，設定: {monthlySetting.MonthlyDay}日 {monthlySetting.MonthlyTime.Value:HH:mm:ss}");
            }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "產生月檢表時發生錯誤");
            }
        }

        private async Task CheckAndGenerateQuarterlyInspectionRecords()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();

                var quarterlySetting = db.InspectionReportTimes.FirstOrDefault(x => x.Type == "Quarterly");
                if (quarterlySetting == null)
                {
                    _logger.LogDebug("季檢設定不存在");
                    return;
                }
                
                if (quarterlySetting.QuarterEnable != true || quarterlySetting.QuarterMonth == null || quarterlySetting.QuarterDay == null || quarterlySetting.QuarterTime == null)
                {
                    _logger.LogDebug("季檢功能已停用或設定不完整");
                    return;
                }

            var now = DateTime.Now;
            int season = (now.Month - 1) / 3 + 1; // 第幾季 (1~4)
            int monthInQuarter = (now.Month - 1) % 3 + 1; // 本季第幾個月 (1~3)
                                                          // 判斷是否到達指定季檢時間（允許誤差3分鐘）
            if (monthInQuarter == quarterlySetting.QuarterMonth &&
                now.Day == quarterlySetting.QuarterDay &&
                quarterlySetting.QuarterTime != null &&
                Math.Abs((now.TimeOfDay - quarterlySetting.QuarterTime.Value).TotalMinutes) < 3)
            {
                var inspections = db.Inspections.Where(x => x.頻率 == "季").ToList();
                var groups = inspections.GroupBy(x => x.機台編號);

                string checknumber = "QI_" + now.ToString("yyyyMMddHH") + "_";

                foreach (var group in groups)
                {
                    foreach (var item in group)
                    {
                        bool exists = db.InspectionRecords.Any(r =>
                            r.機台編號 == item.機台編號 &&
                            r.項目 == item.項目 &&
                            r.檢查單號 == checknumber + item.機台編號 &&
                            r.產生時間.Year == now.Year &&
                            r.產生時間.Month == now.Month
                        );
                        if (exists) continue;

                        var record = new InspectionRecord
                        {
                            機台編號 = item.機台編號,
                            機台名稱 = item.機台名稱,
                            項目 = item.項目,
                            產生時間 = now,
                            檢查人 = "",
                            檢查點位 = item.點檢位置,
                            表單狀態 = InspectionFormStatus.UndoCheck,
                            檢查單號 = checknumber + item.機台編號,
                            結果 = null
                        };
                        db.InspectionRecords.Add(record);
                    }

                    // 產生 InspectionList（每台機台一筆）
                    bool listExists = db.InspectionLists.Any(l =>
                        l.機台編號 == group.Key &&
                        l.產生時間.Year == now.Year &&
                        l.產生時間.Month == now.Month &&
                        l.TYPE == "Quarterly"
                    );
                    if (!listExists)
                    {
                        var inspectionList = new InspectionList
                        {
                            機台編號 = group.Key,
                            機台名稱 = group.First().機台名稱 ?? "",
                            產生時間 = now,
                            TYPE = "Quarterly",
                            表單狀態 = InspectionFormStatus.UndoCheck,
                            檢查人 = "",
                            單號 = checknumber + group.Key
                        };
                        db.InspectionLists.Add(inspectionList);
                    }
                }
                await db.SaveChangesAsync();
                _logger.LogInformation($"成功產生季檢表，時間: {now:yyyy-MM-dd HH:mm:ss}，共 {groups.Count()} 台機台");
            }
            else
            {
                _logger.LogDebug($"尚未到達季檢時間。目前: 第{monthInQuarter}個月 {now.Day}日 {now:HH:mm:ss}，設定: 第{quarterlySetting.QuarterMonth}個月 {quarterlySetting.QuarterDay}日 {quarterlySetting.QuarterTime.Value:HH:mm:ss}");
            }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "產生季檢表時發生錯誤");
            }
        }
        
        private async Task CheckAndGenerateYearlyInspectionRecords()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();

                var yearlySetting = db.InspectionReportTimes.FirstOrDefault(x => x.Type == "Yearly");
                if (yearlySetting == null)
                {
                    _logger.LogDebug("年檢設定不存在");
                    return;
                }
                
                if (yearlySetting.YearEnable != true || yearlySetting.YearMonth == null || yearlySetting.YearDay == null || yearlySetting.YearTime == null)
                {
                    _logger.LogDebug("年檢功能已停用或設定不完整");
                    return;
                }

            var now = DateTime.Now;
            // 判斷是否到達指定年檢時間（允許誤差3分鐘）
            if (now.Month == yearlySetting.YearMonth &&
                now.Day == yearlySetting.YearDay &&
                 Math.Abs((now.TimeOfDay - yearlySetting.YearTime.Value).TotalMinutes) < 3)
            {
                var inspections = db.Inspections.Where(x => x.頻率 == "年").ToList();
                var groups = inspections.GroupBy(x => x.機台編號);

                string checknumber = "YI_" + now.ToString("yyyyMMddHH") + "_";

                foreach (var group in groups)
                {
                    foreach (var item in group)
                    {
                        bool exists = db.InspectionRecords.Any(r =>
                            r.機台編號 == item.機台編號 &&
                            r.項目 == item.項目 &&
                            r.檢查單號 == checknumber + item.機台編號 &&
                            r.產生時間.Year == now.Year &&
                            r.產生時間.Month == now.Month
                        );
                        if (exists) continue;

                        var record = new InspectionRecord
                        {
                            機台編號 = item.機台編號,
                            機台名稱 = item.機台名稱,
                            項目 = item.項目,
                            產生時間 = now,
                            檢查人 = "",
                            檢查點位 = item.點檢位置,
                            表單狀態 = InspectionFormStatus.UndoCheck,
                            檢查單號 = checknumber + item.機台編號,
                            結果 = null
                        };
                        db.InspectionRecords.Add(record);
                    }

                    // 產生 InspectionList（每台機台一筆）
                    bool listExists = db.InspectionLists.Any(l =>
                        l.機台編號 == group.Key &&
                        l.產生時間.Year == now.Year &&
                        l.產生時間.Month == now.Month &&
                        l.TYPE == "Yearly"
                    );
                    if (!listExists)
                    {
                        var inspectionList = new InspectionList
                        {
                            機台編號 = group.Key,
                            機台名稱 = group.First().機台名稱 ?? "",
                            產生時間 = now,
                            TYPE = "Yearly",
                            表單狀態 = InspectionFormStatus.UndoCheck,
                            檢查人 = "",
                            單號 = checknumber + group.Key
                        };
                        db.InspectionLists.Add(inspectionList);
                    }
                }
                await db.SaveChangesAsync();
                _logger.LogInformation($"成功產生年檢表，時間: {now:yyyy-MM-dd HH:mm:ss}，共 {groups.Count()} 台機台");
            }
            else
            {
                _logger.LogDebug($"尚未到達年檢時間。目前: {now.Month}月{now.Day}日 {now:HH:mm:ss}，設定: {yearlySetting.YearMonth}月{yearlySetting.YearDay}日 {yearlySetting.YearTime.Value:HH:mm:ss}");
            }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "產生年檢表時發生錯誤");
            }
        }

        // 將英文星期轉換為中文
        private string GetChineseDayName(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => "星期一",
                DayOfWeek.Tuesday => "星期二",
                DayOfWeek.Wednesday => "星期三",
                DayOfWeek.Thursday => "星期四",
                DayOfWeek.Friday => "星期五",
                DayOfWeek.Saturday => "星期六",
                DayOfWeek.Sunday => "星期日",
                _ => ""
            };
        }

    }
}
