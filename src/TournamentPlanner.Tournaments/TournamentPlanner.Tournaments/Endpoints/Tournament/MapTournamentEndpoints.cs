using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace TournamentPlanner.Tournaments.Endpoints.Tournament;

public static class TournamentEndpointMapping
{
  public static IEndpointRouteBuilder MapTournamentEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/tournaments");

    group.MapGet("/", GetAllTournamentsEndpoint.Handle);
    group.MapPost("/", CreateTournamentEndpoint.Handle);
    group.MapPost(
        "/{tournamentId:guid}/disciplines/{tournamentDisciplineId:guid}/round-robin",
        GenerateRoundRobinScheduleEndpoint.Handle);

    return app;
  }
}
