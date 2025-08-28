using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using CommonLibraryP.MachinePKG.Service;
using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.EntityFrameworkCore;
using CommonLibraryP.MachinePKG;

public class CarbonMonitorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(10); // 每 10 分鐘執行一次

    public CarbonMonitorService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            var machineService = scope.ServiceProvider.GetRequiredService<MachineService>();
            var carbonFactorService = scope.ServiceProvider.GetRequiredService<TaiwanCarbonFactorService>();

            try
            {
                // 取得碳排係數
                var (factor, year, name) = await carbonFactorService.GetLatestAsync("f9bda5d4-e3a1-4634-8ef4-85e0fd619bfa");

                // 取得所有機台
                var machines = await machineService.GetAllMachines();

                foreach (var machine in machines)
                {
                    var tags = await machineService.GetMachineTags(machine.Name);
                    ushort? vinH = tags.FirstOrDefault(t => t.Name.Contains("VIN-H"))?.Value as ushort?;
                    ushort? vinL = tags.FirstOrDefault(t => t.Name.Contains("VIN-L"))?.Value as ushort?;

                    float voltage = 0;
                    if (vinH.HasValue && vinL.HasValue)
                    {
                        // 合併高低位成 32-bit
                        uint raw = ((uint)vinH.Value << 16) | vinL.Value;
                        voltage = BitConverter.Int32BitsToSingle((int)raw);
                    }
                    ushort? iinH = tags.FirstOrDefault(t => t.Name.Contains("IIN-H"))?.Value as ushort?;
                    ushort? iinL = tags.FirstOrDefault(t => t.Name.Contains("IIN-L"))?.Value as ushort?;

                    float current = 0;
                    if (iinH.HasValue && iinL.HasValue)
                    {
                        uint raw = ((uint)iinH.Value << 16) | iinL.Value;
                        current = BitConverter.Int32BitsToSingle((int)raw);
                    }

                    ushort? pwhH = tags.FirstOrDefault(t => t.Name.Contains("PWH-H"))?.Value as ushort?;
                    ushort? pwhL = tags.FirstOrDefault(t => t.Name.Contains("PWH-L"))?.Value as ushort?;

                    float electricity = 0;
                    if (pwhH.HasValue && pwhL.HasValue)
                    {
                        uint raw = ((uint)pwhH.Value << 16) | pwhL.Value;
                        electricity = BitConverter.Int32BitsToSingle((int)raw);
                    }


                    ushort? pwH = tags.FirstOrDefault(t => t.Name.Contains("PW-H"))?.Value as ushort?;
                    ushort? pwL = tags.FirstOrDefault(t => t.Name.Contains("PW-L"))?.Value as ushort?;

                    float ratepower = 0;
                    if (pwH.HasValue && pwL.HasValue)
                    {
                        uint raw = ((uint)pwH.Value << 16) | pwL.Value;
                        ratepower = BitConverter.Int32BitsToSingle((int)raw);
                    }


                    //  double? current = tags.FirstOrDefault(t => t.Name.Contains("Current"))?.Value as double?;
                    // double? voltage = tags.FirstOrDefault(t => t.Name.Contains("Voltage"))?.Value as double?;
                    // double? electricity = tags.FirstOrDefault(t => t.Name.Contains("Electricity"))?.Value as double?;
                    double carbonValue = electricity * factor;

                    var param = new CarbonGeneratorParameter
                    {
                        GeneratorName = machine.Name,
                        Model = "", // 可依需求填入
                        RatedPower = ratepower,
                        CarbonFactor = factor,
                        Current = current,
                        Voltage = voltage,
                        Electricity = electricity,
                        RecordTime = DateTime.Now,
                        Remark = $"Year:{year}, Source:{name}, CarbonValue:{carbonValue}"
                    };
                    db.CarbonGeneratorParameters.Add(param);
                }
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // 可加 log
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}

