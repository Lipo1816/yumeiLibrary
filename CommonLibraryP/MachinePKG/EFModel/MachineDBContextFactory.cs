using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CommonLibraryP.MachinePKG.EFModel
{
    public class MachineDBContextFactory : IDesignTimeDbContextFactory<MachineDBContext>
    {
        public MachineDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MachineDBContext>();
            // 請填入你的實際連線字串
            optionsBuilder.UseSqlServer("Data Source=127.0.0.1;Initial Catalog=TMDB;User ID=sa;Password=P@ssw0rd;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            return new MachineDBContext(optionsBuilder.Options);
        }
    }
}