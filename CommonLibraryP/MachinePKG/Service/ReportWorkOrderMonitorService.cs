using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using CommonLibraryP.MachinePKG.EFModel;

namespace CommonLibraryP.MachinePKG.Service
{
    public class ReportWorkOrderMonitorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

        public ReportWorkOrderMonitorService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
                    var reportService = scope.ServiceProvider.GetRequiredService<ReportWorkOrderService>();

                    // 取得所有已存在的工單號
                    var existingOrderNos = db.ReportWorkOrders.Select(r => r.工單).ToHashSet();

                    // 查詢所有 Workorders
                    var newWorkorders = db.Workorders
                        .Where(w => !existingOrderNos.Contains(w.工單號))
                        .ToList();

                    foreach (var w in newWorkorders)
                    {
                        var entity = new ReportWorkOrder
                        {
                            工單 = w.工單號,
                            狀態 = ReportWorkOrderStatus.undo.ToString(),
                            報工時間 = DateTime.Now,
                            品名 = w.品名, // 假設 Workorders 有品名欄位
                            排產日 = w.排產日, // 假設 Workorders 有排產日欄位
                            報工人員 = "" // 可依需求填入
                        };
                        db.ReportWorkOrders.Add(entity);
                    }

                    if (newWorkorders.Count > 0)
                        await db.SaveChangesAsync();
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
