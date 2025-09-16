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
            _db.EmailSentSettings.Add(setting);
            await _db.SaveChangesAsync();
        }
        public async Task<EmailSentSetting?> GetByIdAsync(string 人員ID)
        {
            return await _db.EmailSentSettings.FirstOrDefaultAsync(x => x.人員ID == 人員ID);
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
        public async Task<List<EmailSentSetting>> GetAllAsync()
        {
            return await _db.EmailSentSettings.ToListAsync();
        }
    }
}
