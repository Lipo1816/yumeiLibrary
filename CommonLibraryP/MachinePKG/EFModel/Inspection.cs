using System.ComponentModel.DataAnnotations;

namespace CommonLibraryP.MachinePKG.EFModel
{
    public class Inspection
    {
        [Key]
        [Required]
        public string 機台編號 { get; set; } = null!; // 主鍵1

        [Key]
        [Required]
        public string 項目 { get; set; } = null!; // 主鍵2

        public string? 機台名稱 { get; set; }
        public string? 點檢位置 { get; set; }

        [Required]
        public string 頻率 { get; set; } = null!; // 不可為 null

        public string? 檢查 { get; set; }
        public string? 標準 { get; set; }
        public string? 方式 { get; set; }
        public string? 紀錄值 { get; set; }
    }
}