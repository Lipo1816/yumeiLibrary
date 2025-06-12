using System.ComponentModel.DataAnnotations;

namespace CommonLibraryP.MachinePKG.EFModel
{
    public class WorkorderCheck
    {
        [Key]
        [Required]
        public string 工單號 { get; set; } = null!; // 主鍵，不可為 null

        [Required]
        public string 料號 { get; set; } = null!;
        [Required]
        public string 品名 { get; set; } = null!;
        public string? 訂單號 { get; set; }
        public string? 點檢單號 { get; set; }
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

        // 新增欄位
        public string? 產品生產SOP1 { get; set; }
        public string? 產品生產SOP2 { get; set; }
        public string? 產品生產SOP3 { get; set; }
        public string? 產品生產SOP4 { get; set; }
        public string? 產品生產SOP5 { get; set; }
        public string? 生產輔具1 { get; set; }
        public string? 生產輔具2 { get; set; }
        public string? 生產輔具3 { get; set; }
        public string? 生產輔具4 { get; set; }
        public string? 生產輔具5 { get; set; }
    }
}
