using Microsoft.Extensions.DependencyInjection;
using TournamentPlanner.Tournaments.Domain.Services;
using TournamentPlanner.Tournaments.Services;

namespace TournamentPlanner.Tournaments.Extensions;

public static class TournamentPlannerServiceCollectionExtensions
{
  public static IServiceCollection AddTournamentPlanner(this IServiceCollection services)
  {
    services.AddScoped<IRoundRobinGenerator, RoundRobinGenerator>();
    services.AddScoped<ITournamentDomainService, TournamentDomainService>();
    return services;
  }
}
