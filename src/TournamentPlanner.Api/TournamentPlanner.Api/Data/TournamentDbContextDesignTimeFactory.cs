using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TournamentPlanner.Data;

namespace TournamentPlanner.Api.Data;

public class TournamentDbContextDesignTimeFactory : IDesignTimeDbContextFactory<TournamentDbContext>
{
  public TournamentDbContext CreateDbContext(string[] args)
  {
    var optionsBuilder = new DbContextOptionsBuilder<TournamentDbContext>();
    optionsBuilder.UseNpgsql("Host=localhost;Database=tournamentdb;Username=postgres;Password=postgres");

    return new TournamentDbContext(optionsBuilder.Options);
  }
}
