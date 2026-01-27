using CommonLibraryP.API;
using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.Service
{
    public class EquipmentSpecLimitService
    {
        private readonly IServiceScopeFactory scopeFactory;

        public EquipmentSpecLimitService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        public async Task<EquipmentSpecLimit?> GetLimitAsync(string machineCode, string itemDescription)
        {
            // 假設您有一個資料庫上下文 dbContext
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();

            return await db.EquipmentSpecLimits
                .FirstOrDefaultAsync(x => x.機台編號 == machineCode && x.機台項目說明 == itemDescription);
        }

        // 取得所有資料
        public async Task<List<EquipmentSpecLimit>> GetAllAsync()
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            return await db.EquipmentSpecLimits.AsNoTracking().ToListAsync();
        }

        // 依 Id 取得單筆
        public async Task<EquipmentSpecLimit?> GetByIdAsync(int id)
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            return await db.EquipmentSpecLimits.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        // 新增或更新
        public async Task<(bool IsSuccess, string Msg)> UpsertAsync(EquipmentSpecLimit data)
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            try
            {
                var target = await db.EquipmentSpecLimits.FirstOrDefaultAsync(x => x.Id == data.Id);
                if (target != null)
                {
                    db.Entry(target).CurrentValues.SetValues(data);
                }
                else
                {
                    await db.EquipmentSpecLimits.AddAsync(data);
                }
                await db.SaveChangesAsync();
                return (true, "儲存成功");
            }
            catch (Exception ex)
            {
                return (false, $"儲存失敗: {ex.Message}");
            }
        }

        // 刪除
        public async Task<(bool IsSuccess, string Msg)> DeleteAsync(int id)
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            try
            {
                var target = await db.EquipmentSpecLimits.FirstOrDefaultAsync(x => x.Id == id);
                if (target != null)
                {
                    db.EquipmentSpecLimits.Remove(target);
                    await db.SaveChangesAsync();
                    return (true, "刪除成功");
                }
                else
                {
                    return (false, "找不到資料");
                }
            }
            catch (Exception ex)
            {
                return (false, $"刪除失敗: {ex.Message}");
            }
        }

        public async Task<RequestResult> UpsertEquipmentSpec(EquipmentSpecLimit spec)
        {
            using var scope = scopeFactory.CreateScope();
            try
            {
                var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                
                // 先嘗試用 Id 查找（如果 Id > 0）
                EquipmentSpecLimit? target = null;
                if (spec.Id > 0)
                {
                    target = await db.EquipmentSpecLimits.FirstOrDefaultAsync(x => x.Id == spec.Id);
                }
                
                // 如果用 Id 找不到，或 Id 為 0，則用業務鍵（機台編號 + 項目）查找
                if (target == null && !string.IsNullOrEmpty(spec.機台編號) && !string.IsNullOrEmpty(spec.項目))
                {
                    target = await db.EquipmentSpecLimits.FirstOrDefaultAsync(x => 
                        x.機台編號 == spec.機台編號 && x.項目 == spec.項目);
                }
                
                bool exist = target is not null;
                if (exist)
                {
                    // 更新現有記錄
                    db.Entry(target).CurrentValues.SetValues(spec);
                }
                else
                {
                    // 新增記錄
                    await db.EquipmentSpecLimits.AddAsync(spec);
                }
                await db.SaveChangesAsync();
                return new(2, $"Upsert EquipmentSpecLimit {spec.機台編號}-{spec.項目} success");
            }
            catch (Exception e)
            {
                return new(4, $"Upsert EquipmentSpecLimit {spec.機台編號}-{spec.項目} fail({e.Message})");
            }
        }
        public async Task<(bool IsSuccess, string Msg)> DeleteAsync(string machineNo, string item)
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            try
            {
                var entity = await db.EquipmentSpecLimits
                    .FirstOrDefaultAsync(x => x.機台編號 == machineNo && x.項目 == item);

                if (entity == null)
                    return (false, "找不到指定資料");

                db.EquipmentSpecLimits.Remove(entity);
                await db.SaveChangesAsync();
                return (true, "刪除成功");
            }
            catch (Exception ex)
            {
                return (false, $"刪除失敗：{ex.Message}");
            }
        }
        public async Task<List<EquipmentSpecLimit>> GetAllEquipmentSpecs()
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            return await db.EquipmentSpecLimits.AsNoTracking().ToListAsync();
        }
        public class TagLimitInfo
        {
            public string MachineCode { get; set; }
            public string MachineName { get; set; }
            public string TagName { get; set; }
            public string ItemDesript { get; set; }
            public double? UpperLimit { get; set; }
            public double? LowerLimit { get; set; }
        }

        // EquipmentSpecLimitService
        public async Task<List<TagLimitInfo>> GetAllWithLimitsAsync()
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();

            var query = db.EquipmentSpecLimits
                .Where(x =>
                    x.電壓上限 != null || x.電壓下限 != null ||
                    x.電流上限 != null || x.電流下限 != null ||
                    x.頻率上限 != null || x.頻率下限 != null ||
                    x.轉速上限 != null || x.轉速下限 != null ||
                    x.水溫上限 != null || x.水溫下限 != null
                )
                .Select(x => new TagLimitInfo
                {
                    MachineCode = x.機台編號,
                    MachineName = x.機台名稱,
                    // Tag Name 的命名原則：{機台編號}_{資訊項目}_{機台項目代碼}
                    TagName = $"{x.機台編號}_{x.資訊項目}_{x.機台項目代碼}",
                    ItemDesript = x.機台項目說明,
                    UpperLimit = (double?)
                        (x.電壓上限 ?? x.電流上限 ?? x.頻率上限 ?? x.轉速上限 ?? x.水溫上限),
                    LowerLimit = (double?)
                        (x.電壓下限 ?? x.電流下限 ?? x.頻率下限 ?? x.轉速下限 ?? x.水溫下限)
                });

            return await query.ToListAsync();
        }

        /// <summary>
        /// 修復被污染的資料：修正機台項目說明、機台項目代碼、說明1 欄位
        /// 並刪除資訊項目記錄數少於4筆的記錄
        /// </summary>
        public async Task<(bool IsSuccess, string Msg, int FixedCount, int DeletedCount)> RepairCorruptedDataAsync()
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            
            try
            {
                // 取得所有記錄，用於分析
                var allRecords = await db.EquipmentSpecLimits.ToListAsync();
                
                // 步驟0：刪除資訊項目記錄數少於4筆的記錄
                var recordsByInfoItem = allRecords
                    .GroupBy(x => new { x.資訊項目, x.機台編號 })
                    .ToList();
                
                int deletedCount = 0;
                var recordsToDelete = new List<EquipmentSpecLimit>();
                
                foreach (var group in recordsByInfoItem)
                {
                    if (group.Count() < 4)
                    {
                        // 記錄數少於4筆，標記為刪除
                        recordsToDelete.AddRange(group);
                        deletedCount += group.Count();
                    }
                }
                
                if (recordsToDelete.Any())
                {
                    db.EquipmentSpecLimits.RemoveRange(recordsToDelete);
                    await db.SaveChangesAsync();
                    System.Diagnostics.Debug.WriteLine($"刪除了 {deletedCount} 筆記錄（資訊項目記錄數少於4筆）");
                }
                
                // 重新取得所有記錄（刪除後）
                allRecords = await db.EquipmentSpecLimits.ToListAsync();

                // 判斷是否為錯誤記錄的函數
                bool IsCorrupted(EquipmentSpecLimit record)
                {
                    // 檢查機台項目說明：單個字母（A, B, C, D, E）或空，或只是"變頻器 電壓"（應該跟隨同一資訊項目組的名稱）
                    bool badDescription = string.IsNullOrEmpty(record.機台項目說明) ||
                                         (record.機台項目說明.Length == 1 && char.IsLetter(record.機台項目說明[0])) ||
                                         record.機台項目說明 == "變頻器 電壓";
                    
                    // 檢查機台項目代碼：IP 地址格式（包含點號）或 "0" 或空
                    bool badCode = string.IsNullOrEmpty(record.機台項目代碼) ||
                                  record.機台項目代碼 == "0" ||
                                  (record.機台項目代碼.Contains(".") && record.機台項目代碼.Split('.').Length >= 3);
                    
                    // 檢查說明1：是 "0" 或空
                    bool badDesc1 = string.IsNullOrEmpty(record.說明1) || record.說明1 == "0";
                    
                    // 如果是"變頻器 電壓"，即使代碼和說明1正確，也要修復
                    if (record.機台項目說明 == "變頻器 電壓")
                        return true;
                    
                    // 如果是單個字母（如 "A"），即使其他條件不完全符合，也要修復
                    if (record.機台項目說明 != null && record.機台項目說明.Length == 1 && char.IsLetter(record.機台項目說明[0]))
                        return true;
                    
                    return badDescription && badCode && badDesc1;
                }

                // 取得所有需要修復的記錄
                var corruptedRecords = allRecords.Where(IsCorrupted).ToList();

                if (!corruptedRecords.Any())
                {
                    string resultMessage = "沒有需要修復的記錄";
                    if (deletedCount > 0)
                    {
                        resultMessage += $"，刪除 {deletedCount} 筆記錄（資訊項目記錄數少於4筆）";
                    }
                    return (true, resultMessage, 0, deletedCount);
                }

                // 調試：記錄需要修復的記錄數量
                System.Diagnostics.Debug.WriteLine($"找到 {corruptedRecords.Count} 筆需要修復的記錄");

                int fixedCount = 0;

                // 按照資訊項目和機台編號分組處理
                var groupedByInfoItem = corruptedRecords
                    .GroupBy(x => new { x.資訊項目, x.機台編號 })
                    .ToList();

                foreach (var group in groupedByInfoItem)
                {
                    string infoItem = group.Key.資訊項目 ?? "";
                    string machineNo = group.Key.機台編號 ?? "";

                    // 取得同一資訊項目的所有記錄（包括正確和錯誤的）
                    var allGroupRecords = allRecords
                        .Where(x => x.資訊項目 == infoItem && x.機台編號 == machineNo)
                        .OrderBy(x => x.Id)
                        .ToList();

                    // 找出正確的記錄（用於推斷正確的值）
                    // 排除"變頻器 電壓"這種錯誤記錄
                    var correctRecords = allGroupRecords
                        .Where(x => !IsCorrupted(x) && x.機台項目說明 != "變頻器 電壓")
                        .ToList();

                    // 從正確的記錄中推斷機台項目說明的基礎部分
                    string baseDescription = "";
                    if (correctRecords.Any())
                    {
                        var firstCorrect = correctRecords.First();
                        baseDescription = firstCorrect.機台項目說明 ?? "";
                        
                        // 移除後綴（如 " 電流"、" 頻率"、" 轉速"、" 電壓"）來取得基礎部分
                        baseDescription = baseDescription
                            .Replace(" 電流", "")
                            .Replace(" 頻率", "")
                            .Replace(" 轉速", "")
                            .Replace(" 電壓", "");
                        
                        // 處理特殊情況：如果包含 "變頻器" 但沒有其他前綴，保留完整描述
                        if (baseDescription.Contains("變頻器") && !baseDescription.Contains("輸高") && 
                            !baseDescription.Contains("輪葉") && !baseDescription.Contains("送風"))
                        {
                            // 檢查是否有其他前綴
                            if (baseDescription.Contains("輸送馬達"))
                                baseDescription = "輸送馬達變頻器";
                            else if (baseDescription.Contains("渦流馬達"))
                                baseDescription = "渦流馬達變頻器";
                            else if (baseDescription.Contains("高壓鼓風馬達"))
                                baseDescription = "高壓鼓風馬達變頻器";
                            else if (baseDescription.Contains("輪葉吹料馬達"))
                                baseDescription = "輪葉吹料馬達變頻器";
                            else if (baseDescription.Contains("刀軸馬達"))
                                baseDescription = "刀軸馬達變頻器";
                            else if (baseDescription.Contains("震動馬達"))
                                baseDescription = "震動馬達變頻器";
                            else if (baseDescription.Contains("滾籠機馬達"))
                                baseDescription = "滾籠機馬達變頻器";
                            else if (baseDescription.Contains("輪葉氣泡馬達"))
                                baseDescription = "輪葉氣泡馬達變頻器";
                            else if (baseDescription.Contains("脫水馬達"))
                                baseDescription = "脫水馬達變頻器";
                            else if (baseDescription.Contains("自轉閥門馬達"))
                                baseDescription = "自轉閥門馬達變頻器";
                            else if (baseDescription.Contains("螺桿機馬達"))
                                baseDescription = "螺桿機馬達變頻器";
                            else if (baseDescription.Contains("送料吹氣馬達"))
                                baseDescription = "送料吹氣馬達變頻器";
                            else if (baseDescription.Contains("送料螺桿馬達"))
                                baseDescription = "送料螺桿馬達變頻器";
                            else if (baseDescription.Contains("風車馬達"))
                                baseDescription = "風車馬達變頻器";
                            else
                                baseDescription = "變頻器";
                        }
                    }
                    else
                    {
                        // 如果沒有正確的記錄，根據資訊項目前綴推斷
                        if (infoItem.StartsWith("IN-GCM"))
                            baseDescription = "輸高馬達變頻器";
                        else if (infoItem.StartsWith("IN-FAN"))
                            baseDescription = "輪葉式送風機變頻器";
                        else if (infoItem.StartsWith("IN-TCM"))
                            baseDescription = "變頻器";
                        else if (infoItem.StartsWith("IN-CUM"))
                            baseDescription = "變頻器";
                        else if (infoItem.StartsWith("IN-MCM"))
                            baseDescription = "變頻器";
                        else if (infoItem.StartsWith("IN-VFCM"))
                            baseDescription = "輸送馬達變頻器";
                        else if (infoItem.StartsWith("IN-VFM"))
                            baseDescription = "渦流馬達變頻器";
                        else if (infoItem.StartsWith("IN-HFM"))
                            baseDescription = "高壓鼓風馬達變頻器";
                        else if (infoItem.StartsWith("IN-WBM"))
                            baseDescription = "輪葉吹料馬達變頻器";
                        else if (infoItem.StartsWith("IN-MCK"))
                            baseDescription = "刀軸馬達變頻器";
                        else if (infoItem.StartsWith("IN-VSM"))
                            baseDescription = "震動馬達變頻器";
                        else if (infoItem.StartsWith("IN-VRM"))
                            baseDescription = "滾籠機馬達變頻器";
                        else if (infoItem.StartsWith("IN-AUC"))
                            baseDescription = "脫水馬達變頻器";
                        else if (infoItem.StartsWith("IN-ATV"))
                            baseDescription = "自轉閥門馬達變頻器";
                        else if (infoItem.StartsWith("IN-SCRM"))
                            baseDescription = "螺桿機馬達變頻器";
                        else if (infoItem.StartsWith("IN-ABM"))
                            baseDescription = "送料吹氣馬達變頻器";
                        else if (infoItem.StartsWith("IN-CM"))
                            baseDescription = "輸送馬達變頻器";
                        else if (infoItem.StartsWith("IN-DCM"))
                            baseDescription = "輸送馬達變頻器";
                        else if (infoItem.StartsWith("IN-FCM"))
                            baseDescription = "輸送馬達變頻器";
                        else if (infoItem.StartsWith("IN-AEM"))
                            baseDescription = "風車馬達變頻器";
                        else if (infoItem.StartsWith("IN-MUC"))
                            baseDescription = "輸送馬達變頻器";
                        else if (infoItem.StartsWith("VF-T"))
                            baseDescription = ""; // 溫度相關，可能需要特殊處理
                        else
                            baseDescription = "變頻器"; // 預設值
                    }

                    // 修復該組的所有錯誤記錄
                    foreach (var record in group)
                    {
                        string finalBaseDescription = baseDescription;
                        
                        // 如果基礎描述為空，使用預設值（根據資訊項目前綴）
                        if (string.IsNullOrEmpty(finalBaseDescription))
                        {
                            // 根據資訊項目前綴推斷（與 else 分支的邏輯相同）
                            if (infoItem.StartsWith("IN-GCM"))
                                finalBaseDescription = "輸高馬達變頻器";
                            else if (infoItem.StartsWith("IN-FAN"))
                                finalBaseDescription = "輪葉式送風機變頻器";
                            else if (infoItem.StartsWith("IN-VFCM"))
                                finalBaseDescription = "輸送馬達變頻器";
                            else if (infoItem.StartsWith("IN-VFM"))
                                finalBaseDescription = "渦流馬達變頻器";
                            else if (infoItem.StartsWith("IN-HFM"))
                                finalBaseDescription = "高壓鼓風馬達變頻器";
                            else if (infoItem.StartsWith("IN-WBM"))
                                finalBaseDescription = "輪葉吹料馬達變頻器";
                            else if (infoItem.StartsWith("IN-MCK"))
                                finalBaseDescription = "刀軸馬達變頻器";
                            else if (infoItem.StartsWith("IN-VSM"))
                                finalBaseDescription = "震動馬達變頻器";
                            else if (infoItem.StartsWith("IN-VRM"))
                                finalBaseDescription = "滾籠機馬達變頻器";
                            else if (infoItem.StartsWith("IN-AUC"))
                                finalBaseDescription = "脫水馬達變頻器";
                            else if (infoItem.StartsWith("IN-ATV"))
                                finalBaseDescription = "自轉閥門馬達變頻器";
                            else if (infoItem.StartsWith("IN-SCRM"))
                                finalBaseDescription = "螺桿機馬達變頻器";
                            else if (infoItem.StartsWith("IN-ABM"))
                                finalBaseDescription = "送料吹氣馬達變頻器";
                            else if (infoItem.StartsWith("IN-CM") && !infoItem.StartsWith("IN-DCM") && !infoItem.StartsWith("IN-FCM") && !infoItem.StartsWith("IN-VFCM"))
                                finalBaseDescription = "輸送馬達變頻器";
                            else if (infoItem.StartsWith("IN-DCM"))
                                finalBaseDescription = "輸送馬達變頻器";
                            else if (infoItem.StartsWith("IN-FCM"))
                                finalBaseDescription = "輸送馬達變頻器";
                            else if (infoItem.StartsWith("IN-AEM"))
                                finalBaseDescription = "風車馬達變頻器";
                            else if (infoItem.StartsWith("IN-MUC"))
                                finalBaseDescription = "輸送馬達變頻器";
                            else if (infoItem.StartsWith("IN-TCM") || infoItem.StartsWith("IN-CUM") || infoItem.StartsWith("IN-MCM"))
                                finalBaseDescription = "變頻器";
                            else
                                finalBaseDescription = "變頻器"; // 預設值
                        }

                        // 修復記錄
                        if (!string.IsNullOrEmpty(finalBaseDescription))
                        {
                            string oldDescription = record.機台項目說明 ?? "";
                            string oldCode = record.機台項目代碼 ?? "";
                            
                            record.機台項目說明 = finalBaseDescription + " 電壓";
                            record.機台項目代碼 = "IN-V";
                            record.說明1 = "電壓(V)";
                            
                            // 如果規格型號是 "0"，設為空字串（因為資料庫不允許 NULL）
                            if (record.規格型號 == "0")
                            {
                                record.規格型號 = "";
                            }

                            db.EquipmentSpecLimits.Update(record);
                            fixedCount++;
                            
                            // 調試：記錄修復的記錄
                            System.Diagnostics.Debug.WriteLine($"修復記錄 Id={record.Id}, 資訊項目={infoItem}, 機台編號={machineNo}, 舊說明={oldDescription}, 舊代碼={oldCode}, 新說明={record.機台項目說明}");
                        }
                        else
                        {
                            // 調試：記錄無法修復的記錄
                            System.Diagnostics.Debug.WriteLine($"無法修復記錄 Id={record.Id}, 資訊項目={infoItem}, 機台編號={machineNo}, 機台項目說明={record.機台項目說明}");
                        }
                    }
                }

                await db.SaveChangesAsync();
                
                string message = $"修復成功，共修復 {fixedCount} 筆記錄";
                if (deletedCount > 0)
                {
                    message += $"，刪除 {deletedCount} 筆記錄（資訊項目記錄數少於4筆）";
                }
                
                return (true, message, fixedCount, deletedCount);
            }
            catch (Exception ex)
            {
                return (false, $"修復失敗: {ex.Message}", 0, 0);
            }
        }
    }
}
