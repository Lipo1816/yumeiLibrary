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
                    ischoose = x.ischoose,
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
            var entity = await _db.EmailSentSettings.FirstOrDefaultAsync(x => x.人員ID == 人員ID);
            if (entity != null)
            {
                entity.ischoose = ischoose;
                entity.建立時間 = DateTime.Now;
                _db.EmailSentSettings.Update(entity);
                await _db.SaveChangesAsync();
            }
        }
        public async Task UpdateAsync(EmailSentSetting setting)
        {
            // 依人員 ID 更新，避免資料表中 Id 欄位為空時導致查詢失敗
            var existing = await _db.EmailSentSettings.FirstOrDefaultAsync(x => x.人員ID == setting.人員ID);
            if (existing != null)
            {
                // 更新所有欄位，Null 一律轉成 false
                existing.ischoose = setting.ischoose;
                existing.生產 = setting.生產 ?? false;
                existing.品管 = setting.品管 ?? false;
                existing.設備 = setting.設備 ?? false;
                existing.環境溫溼度 = setting.環境溫溼度 ?? false;
                existing.建立時間 = setting.建立時間;

                _db.EmailSentSettings.Update(existing);
                await _db.SaveChangesAsync();
            }
        }
        public async Task<List<EmailSentSetting>> GetAllAsync()
        {
            // 取用時即做預設值處理，避免資料庫歷史 Null 值造成 SqlNullValueException
            return await _db.EmailSentSettings
                .AsNoTracking()
                .Select(x => new EmailSentSetting
                {
                    Id = EF.Property<int?>(x, nameof(EmailSentSetting.Id)) ?? 0,
                    人員ID = x.人員ID ?? string.Empty,
                    ischoose = x.ischoose,
                    生產 = x.生產 ?? false,
                    品管 = x.品管 ?? false,
                    設備 = x.設備 ?? false,
                    環境溫溼度 = x.環境溫溼度 ?? false,
                    建立時間 = x.建立時間
                })
                .Where(x => x.Id > 0 && !string.IsNullOrWhiteSpace(x.人員ID))
                .ToListAsync();
        }
    }
}
