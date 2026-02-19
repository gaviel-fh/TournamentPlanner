using Microsoft.AspNetCore.Builder;
using TournamentPlanner.Tournaments.Endpoints.Bout;
using TournamentPlanner.Tournaments.Endpoints.Tournament;

namespace TournamentPlanner.Tournaments.Extensions;

public static class TournamentPlannerApplicationExtensions
{
  public static WebApplication UseTournamentPlanner(this WebApplication app)
  {
    app.MapTournamentEndpoints();
    app.MapBoutEndpoints();
    return app;
  }
}
