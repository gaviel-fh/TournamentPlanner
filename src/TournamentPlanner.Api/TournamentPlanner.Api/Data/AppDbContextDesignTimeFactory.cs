using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TournamentPlanner.Data;

namespace TournamentPlanner.Api.Data;

public class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=tournamentdb;Username=postgres;Password=postgres");

        return new AppDbContext(optionsBuilder.Options);
    }
}
