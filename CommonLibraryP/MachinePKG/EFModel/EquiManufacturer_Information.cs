using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.EFModel
{
  public  class EquiManufacturer_Information
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// PLC項目
        /// </summary>
        public string PLC項目 { get; set; } = string.Empty;

        /// <summary>
        /// 機台名稱
        /// </summary>
        public string 機台名稱 { get; set; } = string.Empty;

        /// <summary>
        /// 機種代碼
        /// </summary>
        public string 機種代碼 { get; set; } = string.Empty;

        /// <summary>
        /// 機台編號
        /// </summary>
        public string 機台編號 { get; set; } = string.Empty;

        /// <summary>
        /// 線別編號
        /// </summary>
        public string 線別編號 { get; set; } = string.Empty;

        /// <summary>
        /// 電控箱
        /// </summary>
        public string 電控箱 { get; set; } = string.Empty;

        /// <summary>
        /// 購入日期
        /// </summary>
        public DateTime? 購入日期 { get; set; }

        /// <summary>
        /// 設備廠商
        /// </summary>
        public string 設備廠商 { get; set; } = string.Empty;

        /// <summary>
        /// 廠商電話
        /// </summary>
        public string 廠商電話 { get; set; } = string.Empty;

        /// <summary>
        /// 廠商MAIL
        /// </summary>
        public string 廠商MAIL { get; set; } = string.Empty;
    }
}
