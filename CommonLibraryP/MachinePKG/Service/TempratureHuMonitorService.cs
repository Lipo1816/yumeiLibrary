using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.Service
{
    public class TempratureHuMonitorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        // 記錄每個設備最後發送郵件的時間（避免一小時內重複寄送）
        private readonly ConcurrentDictionary<string, DateTime> _lastEmailSentTime = new();
        // 記錄每個設備的上一次警報狀態與首次觸發時間（同一 sensor 相同異常沿用同一 FirstAlarmTime）
        private readonly ConcurrentDictionary<string, (bool HasAlarm, string AlarmType, DateTime FirstAlarmTime)> _lastAlarmState = new();
        private static readonly object _logLock = new object();

        public TempratureHuMonitorService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var huService = scope.ServiceProvider.GetRequiredService<TempratureHuService>();
                        var restClient = scope.ServiceProvider.GetRequiredService<RestClient>();
                        var logService = scope.ServiceProvider.GetRequiredService<TempratureHuLogService>();

                        var devices = await huService.GetAllAsync();
                        foreach (var device in devices)
                        {
                            var response = await restClient.GetDeviceDataAsync(device.MachineNumber);
                            if (response != null && response.success && response.device != null)
                            {
                                var temperature = TryParseDouble(response.device.device_data?[0]?.data?[0]?.value);
                                var humidity = TryParseDouble(response.device.device_data?[0]?.data?[1]?.value);
                                
                                var log = new temprature_Hu_log
                                {
                                    MachineName = device.MachineName,
                                    MachineNumber = response.device.id,
                                    MachineGroupNumber = "",
                                    temperature = temperature,
                                    humidity = humidity,
                                    battery = TryParseDouble(response.device.device_data?[0]?.data?[2]?.value),
                                    Status = response.device.status,
                                    CreateDate = DateTime.Now
                                };
                                await logService.UpsertAsync(log);

                                // 檢查溫濕度是否超過門檻並發送警報
                                await CheckAndSendAlarmAsync(scope, device, temperature, humidity);
                            }
                            Thread.Sleep(1000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var huService = scope.ServiceProvider.GetRequiredService<TempratureHuService>();
                        var logService = scope.ServiceProvider.GetRequiredService<TempratureHuLogService>();

                        var devices = await huService.GetAllAsync();
                        foreach (var device in devices)
                        {
                            var lastLog = await logService.GetLastLogByMachineNumber(device.MachineNumber);
                            if (lastLog == null || lastLog.Status != "noConnection")
                            {
                                var log = new temprature_Hu_log
                                {
                                    MachineName = device.MachineName,
                                    MachineNumber = device.MachineNumber,
                                    MachineGroupNumber = "",
                                    temperature = 0,
                                    humidity = 0,
                                    battery = 0,
                                    Status = "noConnection",
                                    CreateDate = DateTime.Now
                                };
                                await logService.UpsertAsync(log);
                            }
                        }
                    }
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                }
            }
        }

        /// <summary>
        /// 檢查溫濕度是否超過門檻，如果超過則發送警報郵件
        /// </summary>
        private async Task CheckAndSendAlarmAsync(IServiceScope scope, temprature_Hu device, double? temperature, double? humidity)
        {
            try
            {
                string alarmType = "";
                bool hasAlarm = false;
                List<string> alarmMessages = new List<string>();

                // 檢查溫度
                if (temperature.HasValue)
                {
                    if (device.temperature_high.HasValue && temperature.Value > device.temperature_high.Value)
                    {
                        hasAlarm = true;
                        alarmType = "TemperatureHigh";
                        alarmMessages.Add($"溫度 {temperature.Value:F2}°C 超過上限 {device.temperature_high.Value:F2}°C");
                    }
                    else if (device.temperature_low.HasValue && temperature.Value < device.temperature_low.Value)
                    {
                        hasAlarm = true;
                        alarmType = "TemperatureLow";
                        alarmMessages.Add($"溫度 {temperature.Value:F2}°C 低於下限 {device.temperature_low.Value:F2}°C");
                    }
                }

                // 檢查濕度
                if (humidity.HasValue)
                {
                    if (device.humidity_high.HasValue && humidity.Value > device.humidity_high.Value)
                    {
                        hasAlarm = true;
                        if (string.IsNullOrEmpty(alarmType))
                            alarmType = "HumidityHigh";
                        else
                            alarmType += "_HumidityHigh";
                        alarmMessages.Add($"濕度 {humidity.Value:F2}% 超過上限 {device.humidity_high.Value:F2}%");
                    }
                    else if (device.humidity_low.HasValue && humidity.Value < device.humidity_low.Value)
                    {
                        hasAlarm = true;
                        if (string.IsNullOrEmpty(alarmType))
                            alarmType = "HumidityLow";
                        else
                            alarmType += "_HumidityLow";
                        alarmMessages.Add($"濕度 {humidity.Value:F2}% 低於下限 {device.humidity_low.Value:F2}%");
                    }
                }

                string alarmKey = device.MachineNumber;
                var lastState = _lastAlarmState.GetValueOrDefault(alarmKey, (HasAlarm: false, AlarmType: "", FirstAlarmTime: DateTime.MinValue));

                if (hasAlarm)
                {
                    DateTime firstAlarmTime;
                    if (lastState.HasAlarm && lastState.AlarmType == alarmType && lastState.FirstAlarmTime != DateTime.MinValue)
                        firstAlarmTime = lastState.FirstAlarmTime;
                    else
                        firstAlarmTime = DateTime.Now;

                    await SendAlarmEmailIfNeededAsync(scope, device, temperature, humidity, alarmMessages, alarmKey, firstAlarmTime);
                    _lastAlarmState[alarmKey] = (HasAlarm: true, AlarmType: alarmType, FirstAlarmTime: firstAlarmTime);
                }
                else if (!hasAlarm && lastState.HasAlarm)
                {
                    await SendRecoveryEmailAsync(scope, device, temperature, humidity, alarmKey, lastState.FirstAlarmTime);
                    _lastAlarmState[alarmKey] = (HasAlarm: false, AlarmType: "", FirstAlarmTime: DateTime.MinValue);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"檢查溫濕度警報時發生錯誤: {ex.Message}");
                Console.WriteLine($"堆疊追蹤: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 發送警報郵件（如果需要的話）；同一設備 1 小時內只寄一次，第 2 小時起會標註「第 N 小時發布」與首次觸發時間（紅字）。
        /// </summary>
        private async Task SendAlarmEmailIfNeededAsync(IServiceScope scope, temprature_Hu device, double? temperature, double? humidity, List<string> alarmMessages, string alarmKey, DateTime firstAlarmTime)
        {
            try
            {
                var lastSentTime = _lastEmailSentTime.GetValueOrDefault(alarmKey, DateTime.MinValue);
                if (DateTime.Now - lastSentTime < TimeSpan.FromHours(1))
                    return;

                var emailSentSettingService = scope.ServiceProvider.GetRequiredService<EmailSentSettingService>();
                var personnalService = scope.ServiceProvider.GetRequiredService<PersonnalService>();
                var emailSettings = await emailSentSettingService.GetAllAsync();
                var allPersonnals = await personnalService.GetAllAsync();
                var recipients = emailSettings
                    .Where(s => s.環境溫溼度 == true)
                    .Join(allPersonnals,
                        setting => setting.人員ID,
                        person => person.人員ID,
                        (setting, person) => new { person.人員姓名, person.Email })
                    .Where(x => !string.IsNullOrWhiteSpace(x.Email))
                    .GroupBy(x => (x.Email ?? "").Trim().ToUpperInvariant())
                    .Select(g => g.First())
                    .ToList();

                if (!recipients.Any())
                {
                    Console.WriteLine($"溫濕度警報：設備 {device.MachineName} 無收件人，跳過發送郵件");
                    LogAlarmEmail("WARN", device.MachineName, alarmKey, "無收件人，跳過發送。");
                    return;
                }

                EmailService? emailService = null;
                try
                {
                    emailService = EmailService.CreateFromConfig();
                }
                catch (Exception configEx)
                {
                    Console.WriteLine($"建立 EmailService 失敗: {configEx.Message}");
                    LogAlarmEmail("ERROR", device.MachineName, alarmKey, $"建立 EmailService 失敗: {configEx.Message}");
                    return;
                }

                if (emailService == null)
                {
                    Console.WriteLine("EmailService 為 null，無法發送郵件");
                    LogAlarmEmail("ERROR", device.MachineName, alarmKey, "EmailService 為 null。");
                    return;
                }

                LogAlarmEmail("INFO", device.MachineName, alarmKey, $"嘗試發送警報 mail，收件人數: {recipients.Count}。");

                var now = DateTime.Now;
                var totalHours = (now - firstAlarmTime).TotalHours;
                int alarmHourIndex = Math.Max(1, (int)Math.Floor(totalHours) + 1);

                var subject = $"溫濕度警報通知 - {device.MachineName}";
                var body = BuildAlarmEmailBody(device, temperature, humidity, alarmMessages, firstAlarmTime, alarmHourIndex);

                // 發送郵件給所有收件人
                bool hasSuccess = false;
                foreach (var recipient in recipients)
                {
                    try
                    {
                        await emailService.SendEmailAsync(
                            fromName: "溫濕度監控系統",
                            fromEmail: "", // 使用配置中的 FromEmail
                            toName: recipient.人員姓名 ?? "收件者",
                            toEmail: recipient.Email!,
                            subject: subject,
                            body: body
                        );
                        hasSuccess = true;
                        Console.WriteLine($"溫濕度警報郵件發送成功 (收件人: {recipient.Email}, 設備: {device.MachineName})");
                        LogAlarmEmail("OK", device.MachineName, alarmKey, $"已發送給: {recipient.Email}");
                    }
                    catch (System.Net.Mail.SmtpException smtpEx)
                    {
                        Console.WriteLine($"發送溫濕度警報郵件失敗 (收件人: {recipient.Email}, 設備: {device.MachineName}): SMTP 錯誤 - {smtpEx.Message}");
                        if (smtpEx.InnerException != null)
                            Console.WriteLine($"內部錯誤: {smtpEx.InnerException.Message}");
                        LogAlarmEmail("FAIL", device.MachineName, alarmKey, $"SMTP 失敗 收件人:{recipient.Email} - {smtpEx.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"發送溫濕度警報郵件失敗 (收件人: {recipient.Email}, 設備: {device.MachineName}): {ex.Message}");
                        Console.WriteLine($"堆疊追蹤: {ex.StackTrace}");
                        LogAlarmEmail("FAIL", device.MachineName, alarmKey, $"發送失敗 收件人:{recipient.Email} - {ex.Message}");
                    }
                }

                if (hasSuccess)
                    _lastEmailSentTime[alarmKey] = DateTime.Now;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發送溫濕度警報郵件時發生未預期的錯誤: {ex.Message}");
                Console.WriteLine($"堆疊追蹤: {ex.StackTrace}");
                if (ex.InnerException != null)
                    Console.WriteLine($"內部錯誤: {ex.InnerException.Message}");
                LogAlarmEmail("ERROR", device.MachineName, alarmKey, $"未預期錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// 構建警報郵件內容（HTML），警報資訊與首次觸發時間、第 N 小時為紅字。
        /// </summary>
        private string BuildAlarmEmailBody(temprature_Hu device, double? temperature, double? humidity, List<string> alarmMessages, DateTime firstAlarmTime, int alarmHourIndex)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html><body style=\"font-family: Microsoft JhengHei, Arial, sans-serif; font-size: 14px;\">");
            sb.AppendLine("<div>溫濕度警報通知</div>");
            sb.AppendLine("<div>====================</div>");
            sb.AppendLine("<br/>");
            sb.AppendLine($"<div>設備名稱: {device.MachineName}</div>");
            sb.AppendLine($"<div>設備編號: {device.MachineNumber}</div>");
            sb.AppendLine("<br/>");
            sb.AppendLine("<div>當前數值:</div>");
            if (temperature.HasValue)
                sb.AppendLine($"<div>&nbsp;&nbsp;溫度: {temperature.Value:F2}°C</div>");
            else
                sb.AppendLine("<div>&nbsp;&nbsp;溫度: 無資料</div>");
            if (humidity.HasValue)
                sb.AppendLine($"<div>&nbsp;&nbsp;濕度: {humidity.Value:F2}%</div>");
            else
                sb.AppendLine("<div>&nbsp;&nbsp;濕度: 無資料</div>");
            sb.AppendLine("<br/>");
            sb.AppendLine("<div>設定範圍:</div>");
            if (device.temperature_high.HasValue || device.temperature_low.HasValue)
            {
                sb.Append("<div>&nbsp;&nbsp;溫度: ");
                if (device.temperature_low.HasValue)
                    sb.Append($"{device.temperature_low.Value:F2}°C ~ ");
                else
                    sb.Append("無下限 ~ ");
                if (device.temperature_high.HasValue)
                    sb.AppendLine($"{device.temperature_high.Value:F2}°C</div>");
                else
                    sb.AppendLine("無上限</div>");
            }
            if (device.humidity_high.HasValue || device.humidity_low.HasValue)
            {
                sb.Append("<div>&nbsp;&nbsp;濕度: ");
                if (device.humidity_low.HasValue)
                    sb.Append($"{device.humidity_low.Value:F2}% ~ ");
                else
                    sb.Append("無下限 ~ ");
                if (device.humidity_high.HasValue)
                    sb.AppendLine($"{device.humidity_high.Value:F2}%</div>");
                else
                    sb.AppendLine("無上限</div>");
            }
            sb.AppendLine("<br/>");
            sb.AppendLine("<div><span style=\"color:red; font-weight:bold;\">警報資訊:</span></div>");
            foreach (var message in alarmMessages)
                sb.AppendLine($"<div><span style=\"color:red;\">&nbsp;&nbsp;⚠ {message}</span></div>");
            if (firstAlarmTime != DateTime.MinValue)
            {
                sb.AppendLine("<br/>");
                sb.AppendLine($"<div><span style=\"color:red;\">警報首次觸發時間: {firstAlarmTime:yyyy-MM-dd HH:mm:ss}</span></div>");
                if (alarmHourIndex > 1)
                    sb.AppendLine($"<div><span style=\"color:red;\">本次郵件為第 {alarmHourIndex} 小時發布，請儘速處理。</span></div>");
            }
            sb.AppendLine("<br/>");
            sb.AppendLine($"<div>警報時間: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</div>");
            sb.AppendLine("<br/>");
            sb.AppendLine("<div>請儘快處理此警報。</div>");
            sb.AppendLine("<br/>");
            sb.AppendLine("<div>此為系統自動發送，請勿回覆。</div>");
            sb.AppendLine("</body></html>");
            return sb.ToString();
        }

        /// <summary>
        /// 警報恢復正常時發送通知（主旨：溫溼度計警報恢復正常通知 + sensor 位置），內容說明為藍字。
        /// </summary>
        private async Task SendRecoveryEmailAsync(IServiceScope scope, temprature_Hu device, double? temperature, double? humidity, string alarmKey, DateTime firstAlarmTime)
        {
            try
            {
                var emailSentSettingService = scope.ServiceProvider.GetRequiredService<EmailSentSettingService>();
                var personnalService = scope.ServiceProvider.GetRequiredService<PersonnalService>();
                var emailSettings = await emailSentSettingService.GetAllAsync();
                var allPersonnals = await personnalService.GetAllAsync();
                var recipients = emailSettings
                    .Where(s => s.環境溫溼度 == true)
                    .Join(allPersonnals,
                        setting => setting.人員ID,
                        person => person.人員ID,
                        (setting, person) => new { person.人員姓名, person.Email })
                    .Where(x => !string.IsNullOrWhiteSpace(x.Email))
                    .GroupBy(x => (x.Email ?? "").Trim().ToUpperInvariant())
                    .Select(g => g.First())
                    .ToList();

                if (!recipients.Any())
                {
                    Console.WriteLine($"溫溼度警報恢復：設備 {device.MachineName} 無收件人，跳過發送郵件");
                    LogAlarmEmail("WARN", device.MachineName, alarmKey, "恢復通知 無收件人，跳過發送。");
                    return;
                }

                EmailService? emailService = null;
                try
                {
                    emailService = EmailService.CreateFromConfig();
                }
                catch (Exception configEx)
                {
                    Console.WriteLine($"建立 EmailService 失敗（恢復通知）: {configEx.Message}");
                    LogAlarmEmail("ERROR", device.MachineName, alarmKey, $"恢復通知 建立 EmailService 失敗: {configEx.Message}");
                    return;
                }

                if (emailService == null)
                {
                    Console.WriteLine("EmailService 為 null，無法發送恢復通知郵件");
                    LogAlarmEmail("ERROR", device.MachineName, alarmKey, "恢復通知 EmailService 為 null。");
                    return;
                }

                LogAlarmEmail("INFO", device.MachineName, alarmKey, $"嘗試發送恢復通知，收件人數: {recipients.Count}。");
                var now = DateTime.Now;
                var body = BuildRecoveryEmailBody(device, temperature, humidity, firstAlarmTime, now);
                var subject = $"溫溼度計警報恢復正常通知 - {device.MachineName}";

                bool hasSuccess = false;
                foreach (var recipient in recipients)
                {
                    try
                    {
                        await emailService.SendEmailAsync(
                            fromName: "溫濕度監控系統",
                            fromEmail: "",
                            toName: recipient.人員姓名 ?? "收件者",
                            toEmail: recipient.Email!,
                            subject: subject,
                            body: body
                        );
                        hasSuccess = true;
                        Console.WriteLine($"溫溼度警報恢復郵件發送成功 (收件人: {recipient.Email}, 設備: {device.MachineName})");
                        LogAlarmEmail("OK", device.MachineName, alarmKey, $"恢復通知 已發送給: {recipient.Email}");
                    }
                    catch (System.Net.Mail.SmtpException smtpEx)
                    {
                        Console.WriteLine($"發送溫溼度警報恢復郵件失敗 (收件人: {recipient.Email}, 設備: {device.MachineName}): SMTP 錯誤 - {smtpEx.Message}");
                        if (smtpEx.InnerException != null)
                            Console.WriteLine($"內部錯誤: {smtpEx.InnerException.Message}");
                        LogAlarmEmail("FAIL", device.MachineName, alarmKey, $"恢復通知 SMTP 失敗 收件人:{recipient.Email} - {smtpEx.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"發送溫溼度警報恢復郵件失敗 (收件人: {recipient.Email}, 設備: {device.MachineName}): {ex.Message}");
                        Console.WriteLine($"堆疊追蹤: {ex.StackTrace}");
                        LogAlarmEmail("FAIL", device.MachineName, alarmKey, $"恢復通知 發送失敗 收件人:{recipient.Email} - {ex.Message}");
                    }
                }

                if (hasSuccess)
                    _lastEmailSentTime[alarmKey] = DateTime.Now;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發送溫溼度警報恢復郵件時發生未預期的錯誤: {ex.Message}");
                Console.WriteLine($"堆疊追蹤: {ex.StackTrace}");
                if (ex.InnerException != null)
                    Console.WriteLine($"內部錯誤: {ex.InnerException.Message}");
                LogAlarmEmail("ERROR", device.MachineName, alarmKey, $"恢復通知 未預期錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// 構建警報恢復郵件內容（HTML），說明使用藍色字體。
        /// </summary>
        private string BuildRecoveryEmailBody(temprature_Hu device, double? temperature, double? humidity, DateTime firstAlarmTime, DateTime recoveryTime)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html><body style=\"font-family: Microsoft JhengHei, Arial, sans-serif; font-size: 14px;\">");
            sb.AppendLine("<div><span style=\"color:blue;\">溫溼度計警報恢復正常通知</span></div>");
            sb.AppendLine("<div>====================</div>");
            sb.AppendLine("<br/>");
            sb.AppendLine($"<div>設備名稱: {device.MachineName}</div>");
            sb.AppendLine($"<div>設備編號: {device.MachineNumber}</div>");
            sb.AppendLine("<br/>");
            sb.AppendLine("<div>當前數值:</div>");
            if (temperature.HasValue)
                sb.AppendLine($"<div>&nbsp;&nbsp;溫度: {temperature.Value:F2}°C</div>");
            else
                sb.AppendLine("<div>&nbsp;&nbsp;溫度: 無資料</div>");
            if (humidity.HasValue)
                sb.AppendLine($"<div>&nbsp;&nbsp;濕度: {humidity.Value:F2}%</div>");
            else
                sb.AppendLine("<div>&nbsp;&nbsp;濕度: 無資料</div>");
            sb.AppendLine("<br/>");
            if (firstAlarmTime != DateTime.MinValue)
            {
                var duration = recoveryTime - firstAlarmTime;
                var totalMinutes = (int)duration.TotalMinutes;
                var hours = totalMinutes / 60;
                var minutes = totalMinutes % 60;
                sb.AppendLine($"<div><span style=\"color:blue;\">警報首次觸發時間: {firstAlarmTime:yyyy-MM-dd HH:mm:ss}</span></div>");
                sb.AppendLine($"<div><span style=\"color:blue;\">警報已於 {recoveryTime:yyyy-MM-dd HH:mm:ss} 恢復正常。</span></div>");
                sb.AppendLine($"<div><span style=\"color:blue;\">警報持續時間約為 {hours} 小時 {minutes} 分鐘。</span></div>");
            }
            else
            {
                sb.AppendLine($"<div><span style=\"color:blue;\">警報已於 {recoveryTime:yyyy-MM-dd HH:mm:ss} 恢復正常。</span></div>");
            }
            sb.AppendLine("<br/>");
            sb.AppendLine("<div><span style=\"color:blue;\">目前溫溼度已回復至設定範圍內，僅供您參考。</span></div>");
            sb.AppendLine("<br/>");
            sb.AppendLine("<div>此為系統自動發送，請勿回覆。</div>");
            sb.AppendLine("</body></html>");
            return sb.ToString();
        }

        /// <summary>
        /// 寫入警報/恢復 mail 日誌，路徑：程式執行目錄下 Logs\AlarmEmail_yyyyMMdd.log
        /// </summary>
        private static void LogAlarmEmail(string level, string deviceName, string machineNumber, string message)
        {
            try
            {
                var logDir = Path.Combine(AppContext.BaseDirectory, "Logs");
                if (!Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);
                var logFile = Path.Combine(logDir, $"AlarmEmail_{DateTime.Now:yyyyMMdd}.log");
                var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] 設備:{deviceName} ({machineNumber}) {message}{Environment.NewLine}";
                lock (_logLock)
                {
                    File.AppendAllText(logFile, line);
                }
            }
            catch { }
        }

        private double? TryParseDouble(string? value)
        {
            if (double.TryParse(value, out var result))
                return result;
            return null;
        }
    }
}
