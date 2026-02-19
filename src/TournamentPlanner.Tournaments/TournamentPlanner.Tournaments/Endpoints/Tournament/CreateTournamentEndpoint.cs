using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TournamentPlanner.Data;
using TournamentPlanner.Data.Entities;
using TournamentPlanner.Tournaments.Contracts;

namespace TournamentPlanner.Tournaments.Endpoints.Tournament;

public static class CreateTournamentEndpoint
{
  public static async Task<IResult> Handle(
      CreateTournamentRequest request,
      TournamentDbContext db,
      CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(request.Name))
    {
      return Results.BadRequest(new { message = "Tournament name is required." });
    }

    if (string.IsNullOrWhiteSpace(request.VenueName))
    {
      return Results.BadRequest(new { message = "Venue name is required." });
    }

    if (request.EndDateUtc < request.StartDateUtc)
    {
      return Results.BadRequest(new { message = "End date must be after start date." });
    }

    if (request.Disciplines.Count == 0)
    {
      return Results.BadRequest(new { message = "At least one discipline is required." });
    }

    var venue = new Venue
    {
      Id = Guid.NewGuid(),
      Name = request.VenueName.Trim()
    };

    var tournament = new Data.Entities.Tournament
    {
      Id = Guid.NewGuid(),
      Name = request.Name.Trim(),
      StartDateUtc = request.StartDateUtc,
      EndDateUtc = request.EndDateUtc,
      VenueId = venue.Id,
      Status = TournamentStatus.Draft
    };

    db.Venues.Add(venue);
    db.Tournaments.Add(tournament);

    AddMembers(tournament.Id, request.OrganizerUserIds, TournamentMemberRole.Organizer, db);
    AddMembers(tournament.Id, request.StaffUserIds, TournamentMemberRole.Staff, db);
    AddMembers(tournament.Id, request.ParticipantUserIds, TournamentMemberRole.Participant, db);

    foreach (var disciplineRequest in request.Disciplines)
    {
      if (string.IsNullOrWhiteSpace(disciplineRequest.Code) || string.IsNullOrWhiteSpace(disciplineRequest.Name))
      {
        return Results.BadRequest(new { message = "Each discipline requires code and name." });
      }

      if (disciplineRequest.RoundCount <= 0)
      {
        return Results.BadRequest(new { message = "Round count must be greater than zero." });
      }

      var normalizedCode = disciplineRequest.Code.Trim().ToUpperInvariant();
      var discipline = await db.Disciplines.FirstOrDefaultAsync(
          d => d.Code == normalizedCode,
          cancellationToken);

      if (discipline is null)
      {
        discipline = new Discipline
        {
          Id = Guid.NewGuid(),
          Code = normalizedCode,
          Name = disciplineRequest.Name.Trim()
        };

        db.Disciplines.Add(discipline);
      }

      db.TournamentDisciplines.Add(new TournamentDiscipline
      {
        Id = Guid.NewGuid(),
        TournamentId = tournament.Id,
        DisciplineId = discipline.Id,
        RoundCount = disciplineRequest.RoundCount,
        BoutIntervalMinutes = disciplineRequest.BoutIntervalMinutes <= 0
              ? 30
              : disciplineRequest.BoutIntervalMinutes
      });
    }

    await db.SaveChangesAsync(cancellationToken);

    return Results.Created($"/tournaments/{tournament.Id}", new { tournament.Id });
  }

  private static void AddMembers(
      Guid tournamentId,
      IEnumerable<Guid> userIds,
      TournamentMemberRole role,
      TournamentDbContext db)
  {
    foreach (var userId in userIds.Where(id => id != Guid.Empty).Distinct())
    {
      db.TournamentMembers.Add(new TournamentMember
      {
        Id = Guid.NewGuid(),
        TournamentId = tournamentId,
        UserId = userId,
        Role = role
      });
    }
  }
}
