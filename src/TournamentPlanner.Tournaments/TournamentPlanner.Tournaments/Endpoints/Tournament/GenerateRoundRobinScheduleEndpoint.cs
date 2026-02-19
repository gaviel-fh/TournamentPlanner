using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TournamentPlanner.Data;
using TournamentPlanner.Data.Entities;
using TournamentPlanner.Tournaments.Contracts;
using TournamentPlanner.Tournaments.Services;

namespace TournamentPlanner.Tournaments.Endpoints.Tournament;

public static class GenerateRoundRobinScheduleEndpoint
{
  public static async Task<IResult> Handle(
      Guid tournamentId,
      Guid tournamentDisciplineId,
      GenerateRoundRobinScheduleRequest request,
      TournamentDbContext db,
      IRoundRobinGenerator roundRobinGenerator,
      CancellationToken cancellationToken)
  {
    var tournamentDiscipline = await db.TournamentDisciplines
        .Include(td => td.Tournament)
        .FirstOrDefaultAsync(
            td => td.Id == tournamentDisciplineId && td.TournamentId == tournamentId,
            cancellationToken);

    if (tournamentDiscipline is null)
    {
      return Results.NotFound(new { message = "Tournament discipline not found." });
    }

    var existingBouts = await db.Bouts
        .AnyAsync(b => b.TournamentDisciplineId == tournamentDiscipline.Id, cancellationToken);

    if (existingBouts)
    {
      return Results.Conflict(new { message = "Schedule already generated for this discipline." });
    }

    var participants = await db.TournamentMembers
        .Where(m => m.TournamentId == tournamentId && m.Role == TournamentMemberRole.Participant)
        .Select(m => m.UserId)
        .Distinct()
        .ToListAsync(cancellationToken);

    if (participants.Count < 2)
    {
      return Results.BadRequest(new { message = "At least two participants are required." });
    }

    var pairings = roundRobinGenerator.GeneratePairings(participants);
    if (pairings.Count == 0)
    {
      return Results.BadRequest(new { message = "Unable to generate pairings." });
    }

    var firstBoutStart = request.FirstBoutStartUtc ?? tournamentDiscipline.Tournament.StartDateUtc;
    var boutIndex = 0;

    foreach (var pairing in pairings)
    {
      var bout = new Data.Entities.Bout
      {
        Id = Guid.NewGuid(),
        TournamentDisciplineId = tournamentDiscipline.Id,
        ParticipantAUserId = pairing.ParticipantA,
        ParticipantBUserId = pairing.ParticipantB,
        ScheduledStartUtc = firstBoutStart.AddMinutes(
              boutIndex * tournamentDiscipline.BoutIntervalMinutes),
        Status = BoutStatus.Scheduled
      };

      db.Bouts.Add(bout);

      for (var roundNumber = 1; roundNumber <= tournamentDiscipline.RoundCount; roundNumber++)
      {
        db.BoutRounds.Add(new BoutRound
        {
          Id = Guid.NewGuid(),
          BoutId = bout.Id,
          RoundNumber = roundNumber,
          ParticipantAScore = 0,
          ParticipantBScore = 0
        });
      }

      boutIndex++;
    }

    await db.SaveChangesAsync(cancellationToken);

    return Results.Ok(new
    {
      CreatedBouts = pairings.Count,
      CreatedRounds = pairings.Count * tournamentDiscipline.RoundCount
    });
  }
}
