using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CommonLibraryP.ResourcePKG
{
    public class ResourceService
    {
        private float cpuUsage = 0;
        public float CPUUsage => cpuUsage;

        private float availableMemory = 0;
        public float AvailableMemory => availableMemory;

        public async Task CheckResource()
        {
            PerformanceCounter cpuCounter = new("Processor", "% Processor Time", "_Total");
            cpuUsage = cpuCounter.NextValue();
            PerformanceCounter availableMemoryCounter = new("Memory", "Available MBytes");
            availableMemory = availableMemoryCounter.NextValue();
            Console.WriteLine($"cpu: {cpuUsage}% memory: {availableMemory}%");
            //await Task.Delay(500);
        }
    }
}
