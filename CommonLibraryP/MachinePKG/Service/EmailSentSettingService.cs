using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.Service
{
    public class EmailSentSettingService
    {
        private readonly MachineDBContext _db;
        public EmailSentSettingService(MachineDBContext db) => _db = db;

        public async Task AddAsync(EmailSentSetting setting)
        {
            // 確保寫入資料時不會留下 Null，避免後續讀取拋出 SqlNullValueException
            setting.生產 ??= false;
            setting.品管 ??= false;
            setting.設備 ??= false;
            setting.環境溫溼度 ??= false;
            setting.ischoose ??= false;

            _db.EmailSentSettings.Add(setting);
            await _db.SaveChangesAsync();
        }
        public async Task<EmailSentSetting?> GetByIdAsync(string 人員ID)
        {
            // 直接用人員 ID 查找，並將可能為 Null 的欄位帶預設值，防止資料異常導致 Blazor 斷線
            return await _db.EmailSentSettings
                .AsNoTracking()
                .Where(x => x.人員ID == 人員ID)
                .Select(x => new EmailSentSetting
                {
                    Id = EF.Property<int?>(x, nameof(EmailSentSetting.Id)) ?? 0,
                    人員ID = x.人員ID ?? string.Empty,
                    ischoose = x.ischoose ?? false,
                    生產 = x.生產 ?? false,
                    品管 = x.品管 ?? false,
                    設備 = x.設備 ?? false,
                    環境溫溼度 = x.環境溫溼度 ?? false,
                    建立時間 = x.建立時間
                })
                .FirstOrDefaultAsync();
        }
        public async Task UpdateAsync(string 人員ID, bool ischoose)
        {
            await _db.EmailSentSettings
                .Where(x => x.人員ID == 人員ID)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(e => e.ischoose, ischoose)
                    .SetProperty(e => e.建立時間, DateTime.Now));
        }

        /// <summary>依人員ID更新，不載入實體，避免 DB 欄位為 NULL 時實體化拋出 SqlNullValueException。</summary>
        public async Task UpdateAsync(EmailSentSetting setting)
        {
            var 人員ID = setting.人員ID ?? "";
            if (string.IsNullOrWhiteSpace(人員ID)) return;

            var ischoose = setting.ischoose ?? false;
            var 生產 = setting.生產 ?? false;
            var 品管 = setting.品管 ?? false;
            var 設備 = setting.設備 ?? false;
            var 環境溫溼度 = setting.環境溫溼度 ?? false;
            var 建立時間 = setting.建立時間 ?? DateTime.Now;

            await _db.EmailSentSettings
                .Where(x => x.人員ID == 人員ID)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(e => e.ischoose, ischoose)
                    .SetProperty(e => e.生產, 生產)
                    .SetProperty(e => e.品管, 品管)
                    .SetProperty(e => e.設備, 設備)
                    .SetProperty(e => e.環境溫溼度, 環境溫溼度)
                    .SetProperty(e => e.建立時間, 建立時間));
        }
        public async Task<List<EmailSentSetting>> GetAllAsync()
        {
            // 取用時即做預設值處理；不以 Id > 0 過濾，避免 DB 的 Id 為 NULL 時新列被排除導致勾選被清空
            return await _db.EmailSentSettings
                .AsNoTracking()
                .Where(x => x.人員ID != null && x.人員ID != "")
                .Select(x => new EmailSentSetting
                {
                    Id = EF.Property<int?>(x, nameof(EmailSentSetting.Id)) ?? 0,
                    人員ID = x.人員ID ?? string.Empty,
                    ischoose = x.ischoose ?? false,
                    生產 = x.生產 ?? false,
                    品管 = x.品管 ?? false,
                    設備 = x.設備 ?? false,
                    環境溫溼度 = x.環境溫溼度 ?? false,
                    建立時間 = x.建立時間
                })
                .ToListAsync();
        }
    }
}
