namespace TournamentPlanner.Tournaments.Contracts;

public record CreateTournamentRequest(
    string Name,
    DateTime StartDateUtc,
    DateTime EndDateUtc,
    DateTime SignupStartDateUtc,
    DateTime SignupEndDateUtc,
    string VenueName,
    double? Latitude,
    double? Longitude,
    IReadOnlyCollection<Guid> OrganizerUserIds,
    IReadOnlyCollection<Guid> StaffUserIds,
    IReadOnlyCollection<Guid> ParticipantUserIds,
    IReadOnlyCollection<CreateTournamentDisciplineRequest> Disciplines);

public record CreateTournamentDisciplineRequest(
    string Code,
    string Name,
    int RoundCount,
    int BoutIntervalMinutes = 30);

public record TournamentSummaryResponse(
    Guid Id,
    string Name,
    DateTime StartDateUtc,
    DateTime EndDateUtc,
    DateTime SignupStartDateUtc,
    DateTime SignupEndDateUtc,
    string Venue,
    double? Latitude,
    double? Longitude,
    int DisciplineCount,
    int ParticipantCount,
    string Status);

public record GenerateRoundRobinScheduleRequest(DateTime? FirstBoutStartUtc = null);

public record ScoreRoundRequest(
    Guid AwardedToUserId,
    Guid AwardedByUserId,
    int Points,
    string? Reason);
