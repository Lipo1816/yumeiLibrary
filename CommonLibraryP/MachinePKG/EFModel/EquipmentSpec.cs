using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public partial class EquipmentSpec
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
        public string PLC讀值型態 { get; set; }
        public string PLC_XY位址 { get; set; }
        public string PLC讀值位址ModbusAdd { get; set; }
        public string 條件或格式 { get; set; }
        public string 電控制箱編號 { get; set; }
        public string 電控制箱IP { get; set; }

    }
}
