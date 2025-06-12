using CommonLibraryP.MachinePKG.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.Service
{
    public class UserSessionService
    {
        private readonly PersonnalService _personnalService;

        public Personnal? CurrentUser { get; set; }
        public string? CurrentUserName => CurrentUser?.人員姓名;

        public UserSessionService(PersonnalService personnalService)
        {
            _personnalService = personnalService;
        }

        // 非同步驗證
        public async Task<Personnal?> AuthenticateAsync(string userId, string password)
        {
            var user = await _personnalService.GetPersonnalByIdAsync(userId);
            if (user != null /* && user.Password == password */) // 需有密碼欄位才可比對
            {
                // 密碼驗證（如有密碼欄位請加上比對）
                return user;
            }
            return null;
        }

        // 取得目前使用者角色
        public string? GetCurrentUserRole()
        {
            return CurrentUser?.權限;
        }

        // 登出
        public void Logout()
        {
            CurrentUser = null;
        }
    }
}
