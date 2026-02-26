using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TournamentPlanner.Data;
using TournamentPlanner.Data.Entities;
using TournamentPlanner.Tournaments.Security;

namespace TournamentPlanner.Tournaments.Endpoints.Tournament;

public static class JoinTournamentEndpoint
{
  public static async Task<IResult> Handle(
      Guid tournamentId,
      TournamentDbContext db,
      HttpContext httpContext,
      CancellationToken cancellationToken)
  {
    var userId = CurrentUserContext.GetUserId(httpContext.User);
    if (userId is null)
    {
      return Results.Unauthorized();
    }

    var tournament = await db.Tournaments
        .AsNoTracking()
        .FirstOrDefaultAsync(t => t.Id == tournamentId, cancellationToken);

    if (tournament is null)
    {
      return Results.NotFound(new { message = "Tournament not found." });
    }

    var nowUtc = DateTime.UtcNow;
    if (nowUtc < tournament.SignupStartDateUtc || nowUtc > tournament.SignupEndDateUtc)
    {
      return Results.BadRequest(new { message = "Signup is currently closed for this tournament." });
    }

    var alreadyMember = await db.TournamentMembers
        .AnyAsync(m => m.TournamentId == tournamentId && m.UserId == userId.Value, cancellationToken);

    if (alreadyMember)
    {
      return Results.Ok(new { joined = false, message = "Already joined." });
    }

    db.TournamentMembers.Add(new TournamentMember
    {
      Id = Guid.NewGuid(),
      TournamentId = tournamentId,
      UserId = userId.Value,
      Role = TournamentMemberRole.Participant
    });

    var hasProfile = await db.TournamentUserProfiles.AnyAsync(p => p.UserId == userId.Value, cancellationToken);
    if (!hasProfile)
    {
      db.TournamentUserProfiles.Add(new TournamentUserProfile { UserId = userId.Value });
    }

    await db.SaveChangesAsync(cancellationToken);

    return Results.Ok(new { joined = true });
  }
}
