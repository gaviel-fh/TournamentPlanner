namespace TournamentPlanner.Tournaments.Domain.Models;

public record TournamentCreationModel(
    string Name,
    DateTime StartDateUtc,
    DateTime EndDateUtc,
    DateTime SignupStartDateUtc,
    DateTime SignupEndDateUtc,
    TournamentLocationModel Location,
    IReadOnlyCollection<Guid> OrganizerUserIds,
    IReadOnlyCollection<Guid> StaffUserIds,
    IReadOnlyCollection<Guid> ParticipantUserIds,
    IReadOnlyCollection<TournamentDisciplineCreationModel> Disciplines);

public record TournamentLocationModel(
    string VenueName,
    double? Latitude,
    double? Longitude);

public record TournamentDisciplineCreationModel(
    string Code,
    string Name,
    int RoundCount,
    int BoutIntervalMinutes);

public record BoutScoreState(
    Guid ParticipantAUserId,
    Guid ParticipantBUserId,
    int ParticipantATotalScore,
    int ParticipantBTotalScore,
    Guid? WinnerUserId);
