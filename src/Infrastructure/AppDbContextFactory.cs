using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure;
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
#if SQLServer
        optionsBuilder.UseSqlServer(args[0]);

#endif
#if PostgreSQL
        optionsBuilder.UseNpgsql(args[0]);

#endif
        optionsBuilder.UseSqlServer(args[0]);
        return new AppDbContext(optionsBuilder.Options);
    }
}
