using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.Service
{
  public  class EmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPassword;
        private readonly bool _enableSsl;
        private readonly string _fromEmail;

        public EmailService(string smtpServer, int smtpPort, string smtpUser, string smtpPassword, bool enableSsl = false, string? fromEmail = null)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _smtpUser = smtpUser;
            _smtpPassword = smtpPassword;
            _enableSsl = enableSsl;
            _fromEmail = fromEmail ?? smtpUser;
        }

        /// <summary>
        /// 從配置文件創建 EmailService 實例
        /// </summary>
        public static EmailService CreateFromConfig()
        {
            var config = LoadConfigFromFile();
            return new EmailService(
                config.SmtpServer,
                config.SmtpPort,
                config.SmtpUser,
                config.SmtpPassword,
                config.EnableSsl,
                config.FromEmail
            );
        }

        /// <summary>
        /// 從配置文件讀取 SMTP 設定參數
        /// </summary>
        public static EmailConfig LoadConfigFromFile()
        {
            var configPath = Path.Combine(AppContext.BaseDirectory, "UserConfig", "emailConfig.txt");
            
            // 預設值
            var config = new EmailConfig
            {
                SmtpServer = "smtp.sinon.com.tw",
                SmtpPort = 25,
                SmtpUser = "yu.mes@yumeifarm.tw",
                SmtpPassword = "SNS@0930",
                EnableSsl = false,
                FromEmail = "yu.mes@yumeifarm.tw"
            };

            if (!File.Exists(configPath))
            {
                // 如果設定檔不存在，使用預設值
                return config;
            }

            try
            {
                var lines = File.ReadAllLines(configPath);
                foreach (var line in lines)
                {
                    var parts = line.Split('=', 2);
                    if (parts.Length != 2) continue;
                    
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();
                    
                    switch (key)
                    {
                        case "SmtpServer":
                            config.SmtpServer = value;
                            break;
                        case "SmtpPort":
                            if (int.TryParse(value, out int port))
                                config.SmtpPort = port;
                            break;
                        case "EmailUser":
                            config.SmtpUser = value;
                            break;
                        case "EmailPassword":
                            config.SmtpPassword = value;
                            break;
                        case "EnableSsl":
                            if (bool.TryParse(value, out bool ssl))
                                config.EnableSsl = ssl;
                            break;
                        case "FromEmail":
                            config.FromEmail = value;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                // 如果讀取失敗，使用預設值
                // 可以考慮記錄日誌
                Console.WriteLine($"讀取 Email 設定失敗：{ex.Message}");
            }

            return config;
        }

        public async Task SendEmailAsync(string fromName, string fromEmail, string toName, string toEmail, string subject, string body)
        {
            await SendEmailAsync(fromName, fromEmail, toName, toEmail, subject, body, null);
        }

        /// <summary>
        /// 發送郵件（支援附件）
        /// </summary>
        /// <param name="fromName">寄件者名稱</param>
        /// <param name="fromEmail">寄件者信箱</param>
        /// <param name="toName">收件者名稱</param>
        /// <param name="toEmail">收件者信箱</param>
        /// <param name="subject">主旨</param>
        /// <param name="body">內容</param>
        /// <param name="attachments">附件檔案路徑列表（可為 null 或空列表）</param>
        public async Task SendEmailAsync(string fromName, string fromEmail, string toName, string toEmail, string subject, string body, IEnumerable<string>? attachments)
        {
            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(_smtpServer))
            {
                throw new ArgumentException("SMTP 主機不能為空");
            }
            
            if (string.IsNullOrWhiteSpace(_smtpUser))
            {
                throw new ArgumentException("使用者帳號不能為空");
            }
            
            if (string.IsNullOrWhiteSpace(_smtpPassword))
            {
                throw new ArgumentException("密碼不能為空");
            }
            
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                throw new ArgumentException("收件者信箱不能為空");
            }

            SmtpClient? client = null;
            MailMessage? mail = null;
            List<Attachment>? attachmentList = null;
            
            try
            {
                client = new SmtpClient(_smtpServer, _smtpPort);
                client.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                client.EnableSsl = _enableSsl;
                client.Timeout = 30000; // 設定 30 秒超時
                
                mail = new MailMessage();
                mail.From = new MailAddress(string.IsNullOrWhiteSpace(fromEmail) ? _fromEmail : fromEmail, fromName);
                mail.To.Add(new MailAddress(toEmail, toName));
                mail.Subject = subject;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.Body = body;
                mail.BodyEncoding = Encoding.UTF8;

                // 如果內容看起來是 HTML，則以 HTML 格式寄出，否則維持純文字
                // 目前溫溼度警報與恢復通知會使用 <html>…</html> 結構
                if (!string.IsNullOrWhiteSpace(body) &&
                    body.IndexOf("<html", StringComparison.OrdinalIgnoreCase) >= 0 &&
                    body.IndexOf("</html>", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    mail.IsBodyHtml = true;
                }
                else
                {
                    mail.IsBodyHtml = false;
                }
                
                // 添加附件
                if (attachments != null && attachments.Any())
                {
                    attachmentList = new List<Attachment>();
                    foreach (var attachmentPath in attachments)
                    {
                        if (string.IsNullOrWhiteSpace(attachmentPath))
                            continue;
                            
                        if (!File.Exists(attachmentPath))
                        {
                            Console.WriteLine($"警告：附件檔案不存在，已跳過：{attachmentPath}");
                            continue;
                        }
                        
                        try
                        {
                            var attachment = new Attachment(attachmentPath);
                            attachmentList.Add(attachment);
                            mail.Attachments.Add(attachment);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"警告：無法添加附件 {attachmentPath}：{ex.Message}");
                        }
                    }
                }
                
                // 使用 SendAsync 而不是 Task.Run，這樣更符合 async/await 模式
                await client.SendMailAsync(mail);
            }
            catch (SmtpException smtpEx)
            {
                var details = $"SMTP 錯誤 ({smtpEx.StatusCode}): {smtpEx.Message}";
                if (smtpEx.InnerException != null)
                {
                    details += $" | 詳細訊息: {smtpEx.InnerException.Message}";
                }
                // 記錄錯誤但不重新拋出，讓調用者決定如何處理
                Console.WriteLine($"EmailService 發送郵件失敗: {details}");
                throw new Exception(details, smtpEx);
            }
            catch (Exception ex)
            {
                var errorMsg = $"寄信失敗：{ex.Message}";
                Console.WriteLine($"EmailService 發送郵件時發生錯誤: {errorMsg}");
                throw new Exception(errorMsg, ex);
            }
            finally
            {
                // 確保資源被正確釋放
                if (attachmentList != null)
                {
                    foreach (var attachment in attachmentList)
                    {
                        attachment?.Dispose();
                    }
                }
                mail?.Dispose();
                client?.Dispose();
            }
        }
    }

    /// <summary>
    /// Email 配置類別
    /// </summary>
    public class EmailConfig
    {
        public string SmtpServer { get; set; } = "smtp.sinon.com.tw";
        public int SmtpPort { get; set; } = 25;
        public string SmtpUser { get; set; } = "yu.mes@yumeifarm.tw";
        public string SmtpPassword { get; set; } = "SNS@0930";
        public bool EnableSsl { get; set; } = false;
        public string FromEmail { get; set; } = "yu.mes@yumeifarm.tw";
    }
}
