using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TournamentPlanner.Data;
using TournamentPlanner.Data.Entities;
using TournamentPlanner.Tournaments.Contracts;

namespace TournamentPlanner.Tournaments.Endpoints.Bout;

public static class ScoreRoundEndpoint
{
  public static async Task<IResult> Handle(
      Guid boutId,
      int roundNumber,
      ScoreRoundRequest request,
      TournamentDbContext db,
      CancellationToken cancellationToken)
  {
    if (request.Points <= 0)
    {
      return Results.BadRequest(new { message = "Points must be greater than zero." });
    }

    var bout = await db.Bouts.FirstOrDefaultAsync(b => b.Id == boutId, cancellationToken);
    if (bout is null)
    {
      return Results.NotFound(new { message = "Bout not found." });
    }

    if (request.AwardedToUserId != bout.ParticipantAUserId && request.AwardedToUserId != bout.ParticipantBUserId)
    {
      return Results.BadRequest(new { message = "Awarded user is not a participant in this bout." });
    }

    var round = await db.BoutRounds.FirstOrDefaultAsync(
        r => r.BoutId == boutId && r.RoundNumber == roundNumber,
        cancellationToken);

    if (round is null)
    {
      return Results.NotFound(new { message = "Round not found." });
    }

    var scoreEvent = new ScoreEvent
    {
      Id = Guid.NewGuid(),
      BoutRoundId = round.Id,
      AwardedByUserId = request.AwardedByUserId,
      AwardedToUserId = request.AwardedToUserId,
      Points = request.Points,
      Reason = request.Reason,
      OccurredAtUtc = DateTime.UtcNow
    };

    db.ScoreEvents.Add(scoreEvent);

    if (request.AwardedToUserId == bout.ParticipantAUserId)
    {
      round.ParticipantAScore += request.Points;
    }
    else
    {
      round.ParticipantBScore += request.Points;
    }

    if (bout.Status == BoutStatus.Scheduled)
    {
      bout.Status = BoutStatus.InProgress;
    }

    await db.SaveChangesAsync(cancellationToken);

    return Results.Ok(new
    {
      round.RoundNumber,
      round.ParticipantAScore,
      round.ParticipantBScore
    });
  }
}
