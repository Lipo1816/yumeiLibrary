using CommonLibraryP.MachinePKG;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.ResourcePKG
{
    public class ResourceHostService :BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        public ResourceHostService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var resourceService = scope.ServiceProvider.GetRequiredService<ResourceService>();
                    await resourceService.CheckResource();
                    await Task.Delay(500);
                }
            }
        }
    }
}
