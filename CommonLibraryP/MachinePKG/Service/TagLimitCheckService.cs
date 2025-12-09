using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonLibraryP.MachinePKG.EFModel;

namespace CommonLibraryP.MachinePKG.Service
{
    public class TagLimitCheckService : BackgroundService
    {
        private readonly ConcurrentDictionary<string, bool> _machineNormalDict = new();
        // 記錄每個機台+標籤的上一次警報狀態（避免重複寫入）
        private readonly ConcurrentDictionary<string, (bool IsAlarm, int? LogId)> _lastAlarmState = new();
        // 記錄每個 tag 最後發送郵件的時間（避免一小時內重複寄送）
        private readonly ConcurrentDictionary<string, DateTime> _lastEmailSentTime = new();
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(10);

        /// <summary>
        /// 取得所有機台的上下限狀態快取（key=機台編號, value=true:正常/false:有超限）
        /// </summary>
        public IReadOnlyDictionary<string, bool> MachineNormalDict => _machineNormalDict;

        public TagLimitCheckService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 立即執行第一次檢查
            await CheckAllAsync(stoppingToken);

            // 之後每 10 分鐘執行一次
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_checkInterval, stoppingToken);
                await CheckAllAsync(stoppingToken);
            }
        }
        // 取得機台是否正常
        public bool IsMachineNormal(string machineCode)
        {
            return _machineNormalDict.TryGetValue(machineCode, out var normal) ? normal : true;
        }

        // 主檢查邏輯
        private async Task CheckAllAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            using var scope = _scopeFactory.CreateScope();
            var limitService = scope.ServiceProvider.GetRequiredService<EquipmentSpecLimitService>();
            var machineService = scope.ServiceProvider.GetRequiredService<MachineService>();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();

            // 1. 取得所有有上下限的 tag 設定
            var tagLimits = await limitService.GetAllWithLimitsAsync(); // 你要實作這個方法
                                                                         // tagLimits 應包含：MachineCode, TagName, 上下限值

            // 2. 依機台分組
            var groupByMachine = tagLimits.GroupBy(x => x.MachineCode);

            foreach (var group in groupByMachine)
            {
                bool allNormal = true;
                foreach (var tagLimit in group)
                {
                    // 3. 取得 tag 即時值
                    var tag = await machineService.GetMachineTag(tagLimit.MachineCode, tagLimit.TagName);
                    if (tag == null || tag.Value == null) continue;

                    if (tag.Value is IConvertible)
                    {
                        double value = Convert.ToDouble(tag.Value);
                        string alarmKey = $"{tagLimit.MachineCode}_{tagLimit.TagName}";
                        bool isOutOfRange = false;
                        string alarmType = "";

                        // 4. 判斷是否超出上下限
                        if (tagLimit.UpperLimit.HasValue && value > tagLimit.UpperLimit.Value)
                        {
                            isOutOfRange = true;
                            alarmType = "UpperLimit";
                            allNormal = false;
                        }
                        else if (tagLimit.LowerLimit.HasValue && value < tagLimit.LowerLimit.Value)
                        {
                            isOutOfRange = true;
                            alarmType = "LowerLimit";
                            allNormal = false;
                        }

                        // 取得上一次的警報狀態
                        var lastState = _lastAlarmState.GetValueOrDefault(alarmKey, (false, null));

                        // 5. 處理警報記錄
                        if (isOutOfRange && !lastState.IsAlarm)
                        {
                            // 新產生警報 -> 寫入資料庫
                            var alarmLog = new TagLimitAlarmLog
                            {
                                MachineCode = tagLimit.MachineCode,
                                MachineName = tagLimit.MachineName,
                                TagName = tagLimit.TagName,
                                TagDescription = tagLimit.ItemDesript,
                                CurrentValue = value,
                                UpperLimit = tagLimit.UpperLimit,
                                LowerLimit = tagLimit.LowerLimit,
                                AlarmType = alarmType,
                                AlarmTime = DateTime.Now,
                                AlarmStatus = "Active"
                            };
                            db.TagLimitAlarmLogs.Add(alarmLog);
                            await db.SaveChangesAsync();

                            // 更新狀態
                            _lastAlarmState[alarmKey] = (true, alarmLog.Id);

                            // 6. 發送郵件通知（檢查是否在一小時內已發送過）
                            await SendAlarmEmailIfNeededAsync(scope, alarmLog, alarmKey);
                        }
                        else if (!isOutOfRange && lastState.IsAlarm && lastState.LogId.HasValue)
                        {
                            // 警報解除 -> 更新資料庫
                            var existingLog = await db.TagLimitAlarmLogs.FindAsync(lastState.LogId.Value);
                            if (existingLog != null && existingLog.AlarmStatus == "Active")
                            {
                                existingLog.AlarmStatus = "Resolved";
                                existingLog.ResolvedTime = DateTime.Now;
                                await db.SaveChangesAsync();
                            }

                            // 更新狀態
                            _lastAlarmState[alarmKey] = (false, null);
                        }
                    }
                }
                _machineNormalDict[group.Key] = allNormal;
            }
        }

        /// <summary>
        /// 發送警報郵件（如果需要的話）
        /// </summary>
        private async Task SendAlarmEmailIfNeededAsync(IServiceScope scope, TagLimitAlarmLog alarmLog, string alarmKey)
        {
            try
            {
                // 檢查是否在一小時內已發送過郵件
                var lastSentTime = _lastEmailSentTime.GetValueOrDefault(alarmKey, DateTime.MinValue);
                if (DateTime.Now - lastSentTime < TimeSpan.FromHours(1))
                {
                    // 一小時內已發送過，跳過
                    return;
                }

                // TODO: 測試用 - 使用固定測試郵件地址
                // 正式環境時，請改回從資料庫查詢收件人列表
                var testRecipient = new { 人員姓名 = "測試收件人", Email = "lipo.lee@tm-robot.com" };
                var recipients = new List<dynamic> { testRecipient };

                // 正式環境的收件人查詢邏輯（暫時註解）
                /*
                // 取得收件人列表（設備警報設定為 true 的人員）
                var emailSentSettingService = scope.ServiceProvider.GetRequiredService<EmailSentSettingService>();
                var personnalService = scope.ServiceProvider.GetRequiredService<PersonnalService>();
                
                var emailSettings = await emailSentSettingService.GetAllAsync();
                var allPersonnals = await personnalService.GetAllAsync();
                
                // 找出設備警報設定為 true 且 Email 不為空的人員
                var recipients = emailSettings
                    .Where(s => s.設備 == true)
                    .Join(allPersonnals,
                        setting => setting.人員ID,
                        person => person.人員ID,
                        (setting, person) => new { person.人員姓名, person.Email })
                    .Where(x => !string.IsNullOrWhiteSpace(x.Email))
                    .ToList();
                */

                if (!recipients.Any())
                {
                    // 沒有收件人，跳過
                    return;
                }

                // 創建 EmailService 實例
                var emailService = EmailService.CreateFromConfig();

                // 構建郵件內容
                var subject = $"設備警報通知 - {alarmLog.MachineName}";
                var body = BuildAlarmEmailBody(alarmLog);

                // 發送郵件給所有收件人
                foreach (var recipient in recipients)
                {
                    try
                    {
                        await emailService.SendEmailAsync(
                            fromName: "設備監控系統",
                            fromEmail: "", // 使用配置中的 FromEmail
                            toName: recipient.人員姓名 ?? "收件者",
                            toEmail: recipient.Email!,
                            subject: subject,
                            body: body
                        );
                    }
                    catch (Exception ex)
                    {
                        // 記錄發送失敗，但不影響其他收件人
                        Console.WriteLine($"發送警報郵件失敗 (收件人: {recipient.Email}): {ex.Message}");
                    }
                }

                // 記錄發送時間
                _lastEmailSentTime[alarmKey] = DateTime.Now;
            }
            catch (Exception ex)
            {
                // 記錄錯誤，但不影響警報記錄
                Console.WriteLine($"發送警報郵件時發生錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// 構建警報郵件內容
        /// </summary>
        private string BuildAlarmEmailBody(TagLimitAlarmLog alarmLog)
        {
            var sb = new StringBuilder();
            sb.AppendLine("設備警報通知");
            sb.AppendLine("====================");
            sb.AppendLine();
            sb.AppendLine($"機台編號: {alarmLog.MachineCode}");
            sb.AppendLine($"機台名稱: {alarmLog.MachineName}");
            sb.AppendLine($"標籤名稱: {alarmLog.TagName}");
            sb.AppendLine($"標籤描述: {alarmLog.TagDescription ?? "無"}");
            sb.AppendLine();
            sb.AppendLine("警報資訊:");
            sb.AppendLine($"  當前值: {alarmLog.CurrentValue:F2}");
            if (alarmLog.UpperLimit.HasValue)
                sb.AppendLine($"  上限值: {alarmLog.UpperLimit.Value:F2}");
            if (alarmLog.LowerLimit.HasValue)
                sb.AppendLine($"  下限值: {alarmLog.LowerLimit.Value:F2}");
            sb.AppendLine($"  警報類型: {(alarmLog.AlarmType == "UpperLimit" ? "超出上限" : "低於下限")}");
            sb.AppendLine($"  警報時間: {alarmLog.AlarmTime:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine();
            sb.AppendLine("請儘快處理此警報。");
            sb.AppendLine();
            sb.AppendLine("此為系統自動發送，請勿回覆。");

            return sb.ToString();
        }
    }
}
