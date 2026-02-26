namespace TournamentPlanner.Tournaments.Contracts;

public record TournamentMemberResponse(
    Guid UserId,
    string DisplayName,
    string Email,
    string? Nickname,
    string Role);

public record TournamentRoundResponse(
    int RoundNumber,
    int ParticipantAScore,
    int ParticipantBScore);

public record TournamentBoutResponse(
    Guid Id,
    DateTime ScheduledStartUtc,
    string Status,
    Guid ParticipantAUserId,
    Guid ParticipantBUserId,
    int ParticipantATotalScore,
    int ParticipantBTotalScore,
    Guid? WinnerUserId,
    IReadOnlyCollection<TournamentRoundResponse> Rounds);

public record TournamentDisciplineDetailResponse(
    Guid TournamentDisciplineId,
    Guid DisciplineId,
    string Code,
    string Name,
    int RoundCount,
    int BoutIntervalMinutes,
    IReadOnlyCollection<TournamentBoutResponse> Bouts);

public record TournamentDetailResponse(
    Guid Id,
    string Name,
    DateTime StartDateUtc,
    DateTime EndDateUtc,
    DateTime SignupStartDateUtc,
    DateTime SignupEndDateUtc,
    string Venue,
    double? Latitude,
    double? Longitude,
    string Status,
    string? CurrentUserRole,
    bool CanJoin,
    bool CanManageStaff,
    bool CanScore,
    IReadOnlyCollection<TournamentMemberResponse> Organizers,
    IReadOnlyCollection<TournamentMemberResponse> Staff,
    IReadOnlyCollection<TournamentMemberResponse> Participants,
    IReadOnlyCollection<TournamentDisciplineDetailResponse> Disciplines);

public record AddStaffRequest(Guid UserId);
