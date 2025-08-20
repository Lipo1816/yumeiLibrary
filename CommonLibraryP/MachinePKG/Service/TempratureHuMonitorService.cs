using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.Service
{
    public class TempratureHuMonitorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public TempratureHuMonitorService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var huService = scope.ServiceProvider.GetRequiredService<TempratureHuService>();
                        var restClient = scope.ServiceProvider.GetRequiredService<RestClient>();
                        var logService = scope.ServiceProvider.GetRequiredService<TempratureHuLogService>();

                        var devices = await huService.GetAllAsync();
                        foreach (var device in devices)
                        {
                            var response = await restClient.GetDeviceDataAsync(device.MachineNumber);
                            if (response != null && response.success && response.device != null)
                            {
                                var log = new temprature_Hu_log
                                {
                                    MachineName = response.device.name,
                                    MachineNumber = response.device.id,
                                    MachineGroupNumber = "",
                                    temperature = TryParseDouble(response.device.device_data?[0]?.data?[0]?.value),
                                    humidity = TryParseDouble(response.device.device_data?[0]?.data?[1]?.value),
                                    battery = TryParseDouble(response.device.device_data?[0]?.data?[2]?.value),
                                    Status = response.device.status,
                                    CreateDate = DateTime.Now
                                };
                                await logService.UpsertAsync(log);
                            }
                            Thread.Sleep(1000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var huService = scope.ServiceProvider.GetRequiredService<TempratureHuService>();
                        var logService = scope.ServiceProvider.GetRequiredService<TempratureHuLogService>();

                        var devices = await huService.GetAllAsync();
                        foreach (var device in devices)
                        {
                            var lastLog = await logService.GetLastLogByMachineNumber(device.MachineNumber);
                            if (lastLog == null || lastLog.Status != "noConnection")
                            {
                                var log = new temprature_Hu_log
                                {
                                    MachineName = device.MachineName,
                                    MachineNumber = device.MachineNumber,
                                    MachineGroupNumber = "",
                                    temperature = 0,
                                    humidity = 0,
                                    battery = 0,
                                    Status = "noConnection",
                                    CreateDate = DateTime.Now
                                };
                                await logService.UpsertAsync(log);
                            }
                        }
                    }
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }

        private double? TryParseDouble(string? value)
        {
            if (double.TryParse(value, out var result))
                return result;
            return null;
        }
    }
}
