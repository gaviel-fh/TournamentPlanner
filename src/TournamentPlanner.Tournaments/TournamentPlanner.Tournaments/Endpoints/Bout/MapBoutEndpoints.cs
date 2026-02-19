using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace TournamentPlanner.Tournaments.Endpoints.Bout;

public static class BoutEndpointMapping
{
  public static IEndpointRouteBuilder MapBoutEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/bouts");

    group.MapPost("/{boutId:guid}/rounds/{roundNumber:int}/score-events", ScoreRoundEndpoint.Handle);
    group.MapPost("/{boutId:guid}/complete", CompleteBoutEndpoint.Handle);

    return app;
  }
}
