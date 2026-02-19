using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TournamentPlanner.Data;

namespace TournamentPlanner.Api.Data;

public class AuthDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=tournamentdb;Username=postgres;Password=postgres");

        return new AuthDbContext(optionsBuilder.Options);
    }
}
