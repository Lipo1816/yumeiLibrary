using System.ComponentModel.DataAnnotations;

namespace CommonLibraryP.MachinePKG.EFModel
{
    public class Workorder
    {
        [Key]
        [Required]
        public string 工單號 { get; set; } = null!; // 主鍵，不可為 null

        [Required]
        public string 料號 { get; set; } = null!;

        [Required]
        public string 品名 { get; set; } = null!;

        public string? 訂單號 { get; set; }

        [Required]
        public decimal 工單發料量 { get; set; }

        [Required]
        public string 生產組別 { get; set; } = null!;

        [Required]
        public string 生產線別 { get; set; } = null!;

        public string? 客戶編號 { get; set; }

        [Required]
        public DateTime 排產日 { get; set; }

        [Required]
        public DateTime 出貨日 { get; set; }

        [Required]
        public int 分盒數 { get; set; }

        [Required]
        public decimal 分盒總重量 { get; set; }

        public string? 製程程式 { get; set; }

        [Required]
        public decimal 標準工時 { get; set; }

        public string? 發料儲位 { get; set; }

        public string? 物料採購單1 { get; set; }
        public string? 物料採購單2 { get; set; }
        public string? 物料採購單3 { get; set; }

        public string? 工單計算方式 { get; set; }

        public DateTime Updatetime { get; set; }
    }
}
