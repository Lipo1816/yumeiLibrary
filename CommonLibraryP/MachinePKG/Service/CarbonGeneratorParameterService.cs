using CommonLibraryP.MachinePKG.EFModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.Service
{
   public class CarbonGeneratorParameterService
    {
        private readonly IDbContextFactory<MachineDBContext> _dbFactory;

        public CarbonGeneratorParameterService(IDbContextFactory<MachineDBContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<IGrouping<string, CarbonGeneratorParameter>>> GetRecentGroupedAsync(int days = 15)
        {
            using var db = _dbFactory.CreateDbContext();
            var fromDate = DateTime.Now.AddDays(-days);
            var data = await db.CarbonGeneratorParameters
                .Where(x => x.RecordTime >= fromDate)
                .OrderByDescending(x => x.RecordTime)
                .ToListAsync();

            return data.GroupBy(x => x.GeneratorName).ToList();
        }
    }
}

