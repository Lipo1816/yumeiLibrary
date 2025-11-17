using System;
using System.ComponentModel.DataAnnotations;

namespace CommonLibraryP.MachinePKG.EFModel
{
    /// <summary>
    /// 設備標籤超限警報記錄
    /// </summary>
    public class TagLimitAlarmLog
    {
        [Key]
        public int Id { get; set; } // 主鍵，自動遞增

        /// <summary>
        /// 機台編號
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string MachineCode { get; set; } = string.Empty;

        /// <summary>
        /// 機台名稱
        /// </summary>
        [MaxLength(100)]
        public string? MachineName { get; set; }

        /// <summary>
        /// 標籤名稱
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string TagName { get; set; } = string.Empty;

        /// <summary>
        /// 標籤項目說明
        /// </summary>
        [MaxLength(200)]
        public string? TagDescription { get; set; }

        /// <summary>
        /// 標籤當前值
        /// </summary>
        public double? CurrentValue { get; set; }

        /// <summary>
        /// 設定的上限值
        /// </summary>
        public double? UpperLimit { get; set; }

        /// <summary>
        /// 設定的下限值
        /// </summary>
        public double? LowerLimit { get; set; }

        /// <summary>
        /// 警報類型：UpperLimit=超出上限, LowerLimit=超出下限
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string AlarmType { get; set; } = string.Empty;

        /// <summary>
        /// 警報發生時間
        /// </summary>
        [Required]
        public DateTime AlarmTime { get; set; }

        /// <summary>
        /// 警報狀態：Active=進行中, Resolved=已解除
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string AlarmStatus { get; set; } = "Active";

        /// <summary>
        /// 警報解除時間
        /// </summary>
        public DateTime? ResolvedTime { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        [MaxLength(500)]
        public string? Remarks { get; set; }
    }
}

