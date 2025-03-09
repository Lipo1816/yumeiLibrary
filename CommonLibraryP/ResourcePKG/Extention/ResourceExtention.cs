using CommonLibraryP.MachinePKG.Service;
using CommonLibraryP.MachinePKG;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CommonLibraryP.ResourcePKG
{
    public static class ResourceExtention
    {
        public static IHostApplicationBuilder AddResourceService(this IHostApplicationBuilder builder)
        {
            builder.Services.AddSingleton<ResourceService>();
            builder.Services.AddHostedService<ResourceHostService>();
            return builder;
        }
    }
}
