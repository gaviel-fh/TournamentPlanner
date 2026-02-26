using TournamentPlanner.Tournaments.Domain.Models;

namespace TournamentPlanner.Tournaments.Contracts;

public static class TournamentContractMappings
{
  public static TournamentCreationModel ToDomain(this CreateTournamentRequest request)
  {
    return new TournamentCreationModel(
        request.Name,
        request.StartDateUtc,
        request.EndDateUtc,
        request.SignupStartDateUtc,
        request.SignupEndDateUtc,
        new TournamentLocationModel(request.VenueName, request.Latitude, request.Longitude),
        request.OrganizerUserIds.Where(id => id != Guid.Empty).Distinct().ToArray(),
        request.StaffUserIds.Where(id => id != Guid.Empty).Distinct().ToArray(),
        request.ParticipantUserIds.Where(id => id != Guid.Empty).Distinct().ToArray(),
        request.Disciplines.Select(d => new TournamentDisciplineCreationModel(
            d.Code,
            d.Name,
            d.RoundCount,
            d.BoutIntervalMinutes <= 0 ? 30 : d.BoutIntervalMinutes)).ToArray());
  }
}
