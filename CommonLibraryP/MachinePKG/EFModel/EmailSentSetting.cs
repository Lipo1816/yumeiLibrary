using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.EFModel
{
    public class EmailSentSetting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string 人員ID { get; set; } = "";

        /// <summary>允許 DB 為 NULL，避免讀取時拋出 SqlNullValueException</summary>
        public bool? ischoose { get; set; }

        public DateTime? 建立時間 { get; set; }

        // 新增且允許 null
        public bool? 生產 { get; set; }
        public bool? 品管 { get; set; }
        public bool? 設備 { get; set; }
        public bool? 環境溫溼度 { get; set; }

    }
}
