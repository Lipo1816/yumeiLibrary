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
    public static double CalculateCarbonEmission(double electricity, double? carbonFactor = null)
    {
        // 若未指定碳排係數，預設 0.474
        double factor = carbonFactor ?? 0.474;
        return electricity * factor;
    }

    public static string? ReadCarboneTokenFromConfig(string configPath)
    {
        if (!File.Exists(configPath))
            return null;

        var lines = File.ReadAllLines(configPath);
        foreach (var line in lines)
        {
            if (line.TrimStart().StartsWith("Carbone_Token:", StringComparison.OrdinalIgnoreCase))
            {
                // 取出冒號後面的內容並去除空白
                return line.Substring("Carbone_Token:".Length).Trim();
            }
        }
        return null;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var configPath = @"E:\玉美\code\CommonLibraryP\CommonLibraryP\MachinePKG\Service\UserConfig\userConfig.txt";
        string carboneToken = ReadCarboneTokenFromConfig(configPath) ?? "f9bda5d4-e3a1-4634-8ef4-85e0fd619bfa";
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MachineDBContext>();
            var machineService = scope.ServiceProvider.GetRequiredService<MachineService>();
            var carbonFactorService = scope.ServiceProvider.GetRequiredService<TaiwanCarbonFactorService>();


                // 取得碳排係數
                // var (factor, year, name) = await carbonFactorService.GetLatestAsync(carboneToken);

                double factor = 0.474; // 預設值
                try
                {
                    var uu = await carbonFactorService.GetTaiwanGridEF_FromMOEAAsync();
                    if (uu > 0) factor = uu;
                }
                catch
                {
                    // 取不到時使用預設值 0.474
                }



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
                        Remark = $"CarbonValue:{carbonValue}"
                    };
                    db.CarbonGeneratorParameters.Add(param);
                }
                await db.SaveChangesAsync();
            }


            await Task.Delay(_interval, stoppingToken);
        }
    }


