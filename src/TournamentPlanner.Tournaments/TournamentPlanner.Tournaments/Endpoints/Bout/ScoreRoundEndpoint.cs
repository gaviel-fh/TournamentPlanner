using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TournamentPlanner.Data;
using TournamentPlanner.Data.Entities;
using TournamentPlanner.Tournaments.Contracts;
using TournamentPlanner.Tournaments.Domain.Models;
using TournamentPlanner.Tournaments.Domain.Services;
using TournamentPlanner.Tournaments.Security;

namespace TournamentPlanner.Tournaments.Endpoints.Bout;

public static class ScoreRoundEndpoint
{
  public static async Task<IResult> Handle(
      Guid boutId,
      int roundNumber,
      ScoreRoundRequest request,
      TournamentDbContext db,
      ITournamentDomainService domainService,
      HttpContext httpContext,
      CancellationToken cancellationToken)
  {
    var currentUserId = CurrentUserContext.GetUserId(httpContext.User);
    if (currentUserId is null)
    {
      return Results.Unauthorized();
    }

    if (request.Points <= 0)
    {
      return Results.BadRequest(new { message = "Points must be greater than zero." });
    }

    var bout = await db.Bouts
        .Include(b => b.TournamentDiscipline)
        .FirstOrDefaultAsync(b => b.Id == boutId, cancellationToken);
    if (bout is null)
    {
      return Results.NotFound(new { message = "Bout not found." });
    }

    var canScore = await db.TournamentMembers.AnyAsync(
        m => m.TournamentId == bout.TournamentDiscipline.TournamentId
             && m.UserId == currentUserId.Value
             && (m.Role == TournamentMemberRole.Organizer || m.Role == TournamentMemberRole.Staff),
        cancellationToken);

    if (!canScore)
    {
      return Results.Forbid();
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
      AwardedByUserId = currentUserId.Value,
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

    var scoreState = new BoutScoreState(
        bout.ParticipantAUserId,
        bout.ParticipantBUserId,
        bout.ParticipantATotalScore,
        bout.ParticipantBTotalScore,
        bout.WinnerUserId);

    var updatedState = domainService.ApplyRoundScore(scoreState, request.AwardedToUserId, request.Points);

    bout.ParticipantATotalScore = updatedState.ParticipantATotalScore;
    bout.ParticipantBTotalScore = updatedState.ParticipantBTotalScore;
    bout.WinnerUserId = updatedState.WinnerUserId;

    if (bout.Status == BoutStatus.Scheduled)
    {
      bout.Status = BoutStatus.InProgress;
    }

    await db.SaveChangesAsync(cancellationToken);

    return Results.Ok(new
    {
      round.RoundNumber,
      round.ParticipantAScore,
      round.ParticipantBScore,
      bout.ParticipantATotalScore,
      bout.ParticipantBTotalScore,
      bout.WinnerUserId
    });
  }
}
