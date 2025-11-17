-- ============================================
-- 檢查 EquipmentSpecLimits 資料表的唯一性約束
-- 確保沒有重複的 (機台編號 + 項目) 組合
-- ============================================

USE [TMDB]
GO

-- 1. 檢查是否有重複的資料
SELECT 
    機台編號, 
    項目, 
    COUNT(*) AS 重複數量
FROM 
    [dbo].[EquipmentSpecLimits]
GROUP BY 
    機台編號, 項目
HAVING 
    COUNT(*) > 1
GO

-- 2. 如果需要，建立唯一索引（可選）
-- 注意：如果資料表中已經有重複資料，此命令會失敗
-- 請先清理重複資料後再執行
/*
IF NOT EXISTS (
    SELECT * FROM sys.indexes 
    WHERE object_id = OBJECT_ID(N'[dbo].[EquipmentSpecLimits]') 
    AND name = N'IX_EquipmentSpecLimits_UniqueKey'
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_EquipmentSpecLimits_UniqueKey]
    ON [dbo].[EquipmentSpecLimits]
    (
        [機台編號] ASC,
        [項目] ASC
    )
    WHERE [機台編號] IS NOT NULL AND [項目] IS NOT NULL
END
GO
*/

-- 3. 查詢最近的記錄
SELECT TOP 10 
    Id,
    機台編號,
    項目,
    機台名稱,
    電壓上限,
    電壓下限,
    電流上限,
    電流下限
FROM 
    [dbo].[EquipmentSpecLimits]
ORDER BY 
    Id DESC
GO

