using CommonLibraryP.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.NotificationUtility
{
    public class NotificationService
    {
        public Func<RequestResult, Task>? ToastAction { get; set; }
        public void ShowToast(RequestResult requestResult)
        {
            ToastAction?.Invoke(requestResult);
        }
    }
}
