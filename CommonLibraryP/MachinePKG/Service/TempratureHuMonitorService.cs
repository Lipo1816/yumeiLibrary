using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        // 記錄每個設備的上一次警報狀態（避免重複發送）
        private readonly ConcurrentDictionary<string, (bool HasAlarm, string AlarmType)> _lastAlarmState = new();

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

                // 取得上一次的警報狀態
                string alarmKey = device.MachineNumber;
                var lastState = _lastAlarmState.GetValueOrDefault(alarmKey, (HasAlarm: false, AlarmType: ""));

                // 如果產生新警報且與上次狀態不同，則發送郵件
                if (hasAlarm && (!lastState.HasAlarm || lastState.AlarmType != alarmType))
                {
                    await SendAlarmEmailIfNeededAsync(scope, device, temperature, humidity, alarmMessages, alarmKey);
                    _lastAlarmState[alarmKey] = (HasAlarm: true, AlarmType: alarmType);
                }
                else if (!hasAlarm && lastState.HasAlarm)
                {
                    // 警報解除，更新狀態
                    _lastAlarmState[alarmKey] = (HasAlarm: false, AlarmType: "");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"檢查溫濕度警報時發生錯誤: {ex.Message}");
                Console.WriteLine($"堆疊追蹤: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 發送警報郵件（如果需要的話）
        /// </summary>
        private async Task SendAlarmEmailIfNeededAsync(IServiceScope scope, temprature_Hu device, double? temperature, double? humidity, List<string> alarmMessages, string alarmKey)
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

                // 取得收件人列表（環境溫溼度設定為 true 的人員）
                var emailSentSettingService = scope.ServiceProvider.GetRequiredService<EmailSentSettingService>();
                var personnalService = scope.ServiceProvider.GetRequiredService<PersonnalService>();
                
                var emailSettings = await emailSentSettingService.GetAllAsync();
                var allPersonnals = await personnalService.GetAllAsync();
                
                // 找出環境溫溼度設定為 true 且 Email 不為空的人員
                var recipients = emailSettings
                    .Where(s => s.環境溫溼度 == true)
                    .Join(allPersonnals,
                        setting => setting.人員ID,
                        person => person.人員ID,
                        (setting, person) => new { person.人員姓名, person.Email })
                    .Where(x => !string.IsNullOrWhiteSpace(x.Email))
                    .ToList();

                if (!recipients.Any())
                {
                    // 沒有收件人，跳過
                    Console.WriteLine($"溫濕度警報：設備 {device.MachineName} 無收件人，跳過發送郵件");
                    return;
                }

                // 創建 EmailService 實例
                EmailService? emailService = null;
                try
                {
                    emailService = EmailService.CreateFromConfig();
                }
                catch (Exception configEx)
                {
                    Console.WriteLine($"建立 EmailService 失敗: {configEx.Message}");
                    Console.WriteLine($"堆疊追蹤: {configEx.StackTrace}");
                    return;
                }

                if (emailService == null)
                {
                    Console.WriteLine("EmailService 為 null，無法發送郵件");
                    return;
                }

                // 構建郵件內容
                var subject = $"溫濕度警報通知 - {device.MachineName}";
                var body = BuildAlarmEmailBody(device, temperature, humidity, alarmMessages);

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
                    }
                    catch (System.Net.Mail.SmtpException smtpEx)
                    {
                        Console.WriteLine($"發送溫濕度警報郵件失敗 (收件人: {recipient.Email}, 設備: {device.MachineName}): SMTP 錯誤 - {smtpEx.Message}");
                        if (smtpEx.InnerException != null)
                        {
                            Console.WriteLine($"內部錯誤: {smtpEx.InnerException.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"發送溫濕度警報郵件失敗 (收件人: {recipient.Email}, 設備: {device.MachineName}): {ex.Message}");
                        Console.WriteLine($"堆疊追蹤: {ex.StackTrace}");
                    }
                }

                // 只有在至少有一封郵件發送成功時，才記錄發送時間
                if (hasSuccess)
                {
                    _lastEmailSentTime[alarmKey] = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發送溫濕度警報郵件時發生未預期的錯誤: {ex.Message}");
                Console.WriteLine($"堆疊追蹤: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"內部錯誤: {ex.InnerException.Message}");
                }
            }
        }

        /// <summary>
        /// 構建警報郵件內容
        /// </summary>
        private string BuildAlarmEmailBody(temprature_Hu device, double? temperature, double? humidity, List<string> alarmMessages)
        {
            var sb = new StringBuilder();
            sb.AppendLine("溫濕度警報通知");
            sb.AppendLine("====================");
            sb.AppendLine();
            sb.AppendLine($"設備名稱: {device.MachineName}");
            sb.AppendLine($"設備編號: {device.MachineNumber}");
            sb.AppendLine();
            sb.AppendLine("當前數值:");
            if (temperature.HasValue)
                sb.AppendLine($"  溫度: {temperature.Value:F2}°C");
            else
                sb.AppendLine($"  溫度: 無資料");
            
            if (humidity.HasValue)
                sb.AppendLine($"  濕度: {humidity.Value:F2}%");
            else
                sb.AppendLine($"  濕度: 無資料");
            
            sb.AppendLine();
            sb.AppendLine("設定範圍:");
            if (device.temperature_high.HasValue || device.temperature_low.HasValue)
            {
                sb.Append($"  溫度: ");
                if (device.temperature_low.HasValue)
                    sb.Append($"{device.temperature_low.Value:F2}°C ~ ");
                else
                    sb.Append("無下限 ~ ");
                if (device.temperature_high.HasValue)
                    sb.AppendLine($"{device.temperature_high.Value:F2}°C");
                else
                    sb.AppendLine("無上限");
            }
            
            if (device.humidity_high.HasValue || device.humidity_low.HasValue)
            {
                sb.Append($"  濕度: ");
                if (device.humidity_low.HasValue)
                    sb.Append($"{device.humidity_low.Value:F2}% ~ ");
                else
                    sb.Append("無下限 ~ ");
                if (device.humidity_high.HasValue)
                    sb.AppendLine($"{device.humidity_high.Value:F2}%");
                else
                    sb.AppendLine("無上限");
            }
            
            sb.AppendLine();
            sb.AppendLine("警報資訊:");
            foreach (var message in alarmMessages)
            {
                sb.AppendLine($"  ⚠ {message}");
            }
            sb.AppendLine();
            sb.AppendLine($"警報時間: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine();
            sb.AppendLine("請儘快處理此警報。");
            sb.AppendLine();
            sb.AppendLine("此為系統自動發送，請勿回覆。");

            return sb.ToString();
        }

        private double? TryParseDouble(string? value)
        {
            if (double.TryParse(value, out var result))
                return result;
            return null;
        }
    }
}
