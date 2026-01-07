using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CommonLibraryP.LogPKG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public class MachineInitHostingService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<MachineInitHostingService>? _logger;
        
        public MachineInitHostingService(IServiceScopeFactory scopeFactory, ILogger<MachineInitHostingService>? logger = null)
        {
            this.scopeFactory = scopeFactory;
            this._logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var machineService = scope.ServiceProvider.GetRequiredService<MachineService>();
                    await machineService.InitAllModbusSlaves();
                    //SerilogByNamespaceRollbyday.Information("InitAllModbusSlaves");
                    await machineService.InitAllMachinesFromDB();
                }
            }
            catch (OperationCanceledException)
            {
                // 正常取消，不需要記錄錯誤
            }
            catch (Exception ex)
            {
                // 記錄初始化錯誤，但不重新拋出，避免導致應用程序關閉
                _logger?.LogError(ex, "MachineInitHostingService 初始化時發生錯誤");
            }
            
            // 等待取消信號，避免服務立即結束
            try
            {
                await Task.Delay(-1, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // 正常取消
            }
        }
    }
}
