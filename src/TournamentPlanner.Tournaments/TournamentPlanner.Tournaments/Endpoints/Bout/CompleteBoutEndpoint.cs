using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TournamentPlanner.Data;
using TournamentPlanner.Data.Entities;

namespace TournamentPlanner.Tournaments.Endpoints.Bout;

public static class CompleteBoutEndpoint
{
  public static async Task<IResult> Handle(
      Guid boutId,
      TournamentDbContext db,
      CancellationToken cancellationToken)
  {
    var bout = await db.Bouts
        .Include(b => b.Rounds)
        .FirstOrDefaultAsync(b => b.Id == boutId, cancellationToken);

    if (bout is null)
    {
      return Results.NotFound(new { message = "Bout not found." });
    }

    if (bout.Rounds.Count == 0)
    {
      return Results.BadRequest(new { message = "No rounds found for this bout." });
    }

    var participantATotal = bout.ParticipantATotalScore;
    var participantBTotal = bout.ParticipantBTotalScore;

    if (participantATotal == 0 && participantBTotal == 0)
    {
      participantATotal = bout.Rounds.Sum(r => r.ParticipantAScore);
      participantBTotal = bout.Rounds.Sum(r => r.ParticipantBScore);
      bout.ParticipantATotalScore = participantATotal;
      bout.ParticipantBTotalScore = participantBTotal;
    }

    bout.Status = BoutStatus.Completed;
    bout.WinnerUserId = participantATotal == participantBTotal
        ? null
        : participantATotal > participantBTotal
            ? bout.ParticipantAUserId
            : bout.ParticipantBUserId;

    await db.SaveChangesAsync(cancellationToken);

    return Results.Ok(new
    {
      bout.Id,
      participantATotal,
      participantBTotal,
      bout.WinnerUserId,
      bout.Status
    });
  }
}
