using CommonLibraryP.MachinePKG;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.ShopfloorPKG.Service
{
    public class ShopfloorInitHostingService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        public ShopfloorInitHostingService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var shopfloorService = scope.ServiceProvider.GetRequiredService<ShopfloorService>();
                await shopfloorService.InitAllStation();
            }
        }
    }
}
