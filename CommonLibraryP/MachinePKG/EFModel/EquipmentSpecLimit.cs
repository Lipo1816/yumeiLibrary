using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.EFModel
{
    public class EquipmentSpecLimit
    {
        public int Id { get; set; }
        public string 項目 { get; set; }
        public string 機台名稱 { get; set; }
        public string 機種代碼 { get; set; }
        public string 機台編號 { get; set; }
        public string 線別編號 { get; set; }
        public string 資訊項目 { get; set; }
        public string 機台項目說明 { get; set; }
        public string 機台項目代碼 { get; set; }
        public string 規格型號 { get; set; }
        public string 說明1 { get; set; }
        public string 電控制箱編號 { get; set; }
        public string 電控制箱IP { get; set; }

        // 新增上下限欄位
        public decimal? 電壓上限 { get; set; }
        public decimal? 電壓下限 { get; set; }
        public decimal? 電流上限 { get; set; }
        public decimal? 電流下限 { get; set; }
        public decimal? 頻率上限 { get; set; }
        public decimal? 頻率下限 { get; set; }
        public decimal? 轉速上限 { get; set; }
        public decimal? 轉速下限 { get; set; }
        public decimal? 水溫上限 { get; set; }
        public decimal? 水溫下限 { get; set; }
    }
}
