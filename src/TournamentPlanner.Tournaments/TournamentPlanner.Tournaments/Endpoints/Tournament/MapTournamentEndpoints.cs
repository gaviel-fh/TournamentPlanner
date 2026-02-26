using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace TournamentPlanner.Tournaments.Endpoints.Tournament;

public static class TournamentEndpointMapping
{
  public static IEndpointRouteBuilder MapTournamentEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/tournaments").RequireAuthorization();

    group.MapGet("/", GetAllTournamentsEndpoint.Handle);
    group.MapGet("/{tournamentId:guid}", GetTournamentDetailEndpoint.Handle);
    group.MapPost("/", CreateTournamentEndpoint.Handle);
    group.MapPost("/{tournamentId:guid}/join", JoinTournamentEndpoint.Handle);
    group.MapPost("/{tournamentId:guid}/staff", AddTournamentStaffEndpoint.Handle);
    group.MapPost(
        "/{tournamentId:guid}/disciplines/{tournamentDisciplineId:guid}/round-robin",
        GenerateRoundRobinScheduleEndpoint.Handle);

    return app;
  }
}
