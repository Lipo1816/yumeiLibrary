using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.EFModel
{
    public class Personnal
    {
        [Key] // 設為主鍵
        public string 人員ID { get; set; }
        public string? 部門ID { get; set; }
        public string? 部門名稱 { get; set; }
        public string? 生產組名 { get; set; }
        public string? 人員姓名 { get; set; }
        public string? 職級代號 { get; set; }
        public string? 職級名稱 { get; set; }
        public string? Email { get; set; }
        public string? 權限 { get; set; }
        public string? 權限頁面 { get; set; }
    }
}
