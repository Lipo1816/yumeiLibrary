using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

        public InspectionMonitorService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }



        private async Task CheckAndGenerateDailyInspectionRecords()
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();

            // 取得日檢時間
            var dailyTime = db.InspectionReportTimes.FirstOrDefault(x => x.Type == "Daily")?.DailyTime;
            if (dailyTime == null) return;

            var now = DateTime.Now;
            // 判斷是否到達日檢時間（允許誤差3分鐘）
            if (now.TimeOfDay.Hours == dailyTime.Value.Hours && Math.Abs(now.TimeOfDay.Minutes - dailyTime.Value.Minutes) < 3)
            {
                var inspections = db.Inspections.Where(x => x.頻率 == "日").ToList();
                var groups = inspections.GroupBy(x => x.機台編號);

                string checknumber = "DI_" + now.ToString("yyyyMMddHH") + "_";

                foreach (var group in groups)
                {
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
                            檢查單號 = checknumber + item.機台編號
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
            }
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckAndGenerateDailyInspectionRecords();
                await CheckAndGenerateWeeklyInspectionRecords();
                await CheckAndGenerateMonthlyInspectionRecords();
                await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken); // 每分鐘檢查一次
            }
        }

        private async Task CheckAndGenerateWeeklyInspectionRecords()
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();

            var weeklySetting = db.InspectionReportTimes.FirstOrDefault(x => x.Type == "Weekly");
            if (weeklySetting?.WeeklyDay == null || weeklySetting.WeeklyTime == null) return;

            var now = DateTime.Now;
            // 判斷是否到達指定周檢時間（允許誤差3分鐘）
            if (now.DayOfWeek.ToString() == weeklySetting.WeeklyDay &&
                now.TimeOfDay.Hours == weeklySetting.WeeklyTime.Value.Hours)
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
                            檢查單號 = checknumber + item.機台編號
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
            }
        }



        private async Task CheckAndGenerateMonthlyInspectionRecords()
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();

            var monthlySetting = db.InspectionReportTimes.FirstOrDefault(x => x.Type == "Monthly");
            if (monthlySetting?.MonthlyDay == null || monthlySetting.MonthlyTime == null) return;

            var now = DateTime.Now;
            // 判斷是否到達指定月檢時間（允許誤差3分鐘）
            if (now.Day == monthlySetting.MonthlyDay &&
                now.TimeOfDay.Hours == monthlySetting.MonthlyTime.Value.Hours )
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
                            表單狀態 = InspectionFormStatus.UndoCheck,
                            檢查單號 = checknumber + item.機台編號
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
            }
        }
    }
}
