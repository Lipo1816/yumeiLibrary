-- ============================================
-- 創建 TagLimitAlarmLogs 資料表
-- 用於記錄設備標籤超限警報
-- ============================================

USE [TMDB]
GO

-- 如果資料表已存在，先刪除
IF OBJECT_ID('dbo.TagLimitAlarmLogs', 'U') IS NOT NULL
    DROP TABLE [dbo].[TagLimitAlarmLogs]
GO

-- 創建資料表
CREATE TABLE [dbo].[TagLimitAlarmLogs](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [MachineCode] [nvarchar](100) NOT NULL,
    [MachineName] [nvarchar](100) NULL,
    [TagName] [nvarchar](100) NOT NULL,
    [TagDescription] [nvarchar](200) NULL,
    [CurrentValue] [float] NULL,
    [UpperLimit] [float] NULL,
    [LowerLimit] [float] NULL,
    [AlarmType] [nvarchar](20) NOT NULL,
    [AlarmTime] [datetime2](7) NOT NULL,
    [AlarmStatus] [nvarchar](20) NOT NULL DEFAULT 'Active',
    [ResolvedTime] [datetime2](7) NULL,
    [Remarks] [nvarchar](500) NULL,
    CONSTRAINT [PK_TagLimitAlarmLogs] PRIMARY KEY CLUSTERED ([Id] ASC)
        WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, 
              ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) 
        ON [PRIMARY]
) ON [PRIMARY]
GO

-- 創建索引以提升查詢效能
CREATE NONCLUSTERED INDEX [IX_TagLimitAlarmLogs_MachineCode] 
    ON [dbo].[TagLimitAlarmLogs]([MachineCode] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_TagLimitAlarmLogs_AlarmTime] 
    ON [dbo].[TagLimitAlarmLogs]([AlarmTime] DESC)
GO

CREATE NONCLUSTERED INDEX [IX_TagLimitAlarmLogs_AlarmStatus] 
    ON [dbo].[TagLimitAlarmLogs]([AlarmStatus] ASC)
GO

PRINT '資料表 TagLimitAlarmLogs 創建成功！'
GO

