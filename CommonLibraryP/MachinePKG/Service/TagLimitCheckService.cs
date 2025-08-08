using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.Service
{
    public class TagLimitCheckService : IDisposable
    {
        private readonly EquipmentSpecLimitService _limitService;
        private readonly MachineService _machineService;
        private readonly Timer _timer;
        private readonly ConcurrentDictionary<string, bool> _machineNormalDict = new();
        private readonly IServiceScopeFactory _scopeFactory;
        /// <summary>
        /// 取得所有機台的上下限狀態快取（key=機台編號, value=true:正常/false:有超限）
        /// </summary>
        public IReadOnlyDictionary<string, bool> MachineNormalDict => _machineNormalDict;
        //public TagLimitCheckService(EquipmentSpecLimitService limitService, MachineService machineService)
        //{
        //    _limitService = limitService;
        //    _machineService = machineService;
        //    _timer = new Timer(async _ => await CheckAllAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        //}
        public TagLimitCheckService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _timer = new Timer(async _ => await CheckAllAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }
        // 取得機台是否正常
        public bool IsMachineNormal(string machineCode)
        {
            return _machineNormalDict.TryGetValue(machineCode, out var normal) ? normal : true;
        }

        // 主檢查邏輯
        private async Task CheckAllAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var limitService = scope.ServiceProvider.GetRequiredService<EquipmentSpecLimitService>();
            var machineService = scope.ServiceProvider.GetRequiredService<MachineService>();
            // 1. 取得所有有上下限的 tag 設定
            var tagLimits = await limitService.GetAllWithLimitsAsync(); // 你要實作這個方法
                                                                         // tagLimits 應包含：MachineCode, TagName, 上下限值

            // 2. 依機台分組
            var groupByMachine = tagLimits.GroupBy(x => x.MachineCode);

            foreach (var group in groupByMachine)
            {
                bool allNormal = true;
                foreach (var tagLimit in group)
                {
                    // 3. 取得 tag 即時值
                    var tag = await machineService.GetMachineTag(tagLimit.MachineCode, tagLimit.TagName);
                    if (tag == null || tag.Value == null) continue;

                    if (tag.Value is IConvertible)
                    {
                        double value = Convert.ToDouble(tag.Value);
                        // 4. 判斷是否超出上下限
                        if ((tagLimit.LowerLimit.HasValue && value < tagLimit.LowerLimit.Value) ||
                            (tagLimit.UpperLimit.HasValue && value > tagLimit.UpperLimit.Value))
                        {
                            allNormal = false;
                            break;
                        }
                    }
                }
                _machineNormalDict[group.Key] = allNormal;
            }
        }



        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
