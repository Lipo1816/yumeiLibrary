using CommonLibraryP.MachinePKG;
using CommonLibraryP.ShopfloorPKG.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.ShopfloorPKG
{
    public static class ShopfloorExtention
    {
        public static IHostApplicationBuilder AddShopfloorService(this IHostApplicationBuilder builder, string dbConnectionStringName = "DefaultConnection")
        {
            builder.Services.AddDbContextFactory<ShopfloorDBContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString(dbConnectionStringName));
            });
            builder.Services.AddSingleton<ShopfloorService>();
            builder.Services.AddHostedService<ShopfloorInitHostingService>();
            builder.Services.AddLocalization();
            return builder;
        }
    }
}
