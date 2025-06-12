using System.ComponentModel.DataAnnotations;

namespace CommonLibraryP.MachinePKG.EFModel
{
    public class ProblemDescript
    {
        [Key]
        public string 不良代碼 { get; set; } = ""; // 主鍵

        public string? 不良類型代碼 { get; set; }
        public string? 不良類型 { get; set; }
        public string? 不良描述 { get; set; }
    }
}
