using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TournamentPlanner.Data;
using TournamentPlanner.Data.Entities;
using TournamentPlanner.Tournaments.Contracts;
using TournamentPlanner.Tournaments.Domain.Services;

namespace TournamentPlanner.Tournaments.Endpoints.Tournament;

public static class CreateTournamentEndpoint
{
  public static async Task<IResult> Handle(
      CreateTournamentRequest request,
      TournamentDbContext db,
      ITournamentDomainService domainService,
      CancellationToken cancellationToken)
  {
    var model = request.ToDomain();
    var validationErrors = domainService.ValidateTournamentCreation(model);
    if (validationErrors.Count > 0)
    {
      return Results.BadRequest(new { errors = validationErrors });
    }

    var venue = new Venue
    {
      Id = Guid.NewGuid(),
      Name = model.Location.VenueName.Trim(),
      Latitude = model.Location.Latitude,
      Longitude = model.Location.Longitude
    };

    var tournament = new Data.Entities.Tournament
    {
      Id = Guid.NewGuid(),
      Name = model.Name.Trim(),
      StartDateUtc = model.StartDateUtc,
      EndDateUtc = model.EndDateUtc,
      SignupStartDateUtc = model.SignupStartDateUtc,
      SignupEndDateUtc = model.SignupEndDateUtc,
      VenueId = venue.Id,
      Status = TournamentStatus.Draft
    };

    db.Venues.Add(venue);
    db.Tournaments.Add(tournament);

    AddMembers(tournament.Id, model.OrganizerUserIds, TournamentMemberRole.Organizer, db);
    AddMembers(tournament.Id, model.StaffUserIds, TournamentMemberRole.Staff, db);
    AddMembers(tournament.Id, model.ParticipantUserIds, TournamentMemberRole.Participant, db);

    var profileUserIds = model.OrganizerUserIds
        .Concat(model.StaffUserIds)
        .Concat(model.ParticipantUserIds)
        .Distinct()
        .ToArray();

    var existingProfileUserIds = await db.TournamentUserProfiles
        .Where(p => profileUserIds.Contains(p.UserId))
        .Select(p => p.UserId)
        .ToListAsync(cancellationToken);

    foreach (var userId in profileUserIds.Except(existingProfileUserIds))
    {
      db.TournamentUserProfiles.Add(new TournamentUserProfile
      {
        UserId = userId
      });
    }

    foreach (var disciplineRequest in model.Disciplines)
    {
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
