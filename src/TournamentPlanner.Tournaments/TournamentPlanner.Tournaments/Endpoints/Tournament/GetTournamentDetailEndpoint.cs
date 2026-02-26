using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TournamentPlanner.Data;
using TournamentPlanner.Data.Entities;
using TournamentPlanner.Tournaments.Contracts;
using TournamentPlanner.Tournaments.Security;

namespace TournamentPlanner.Tournaments.Endpoints.Tournament;

public static class GetTournamentDetailEndpoint
{
  public static async Task<IResult> Handle(
      Guid tournamentId,
      TournamentDbContext tournamentDb,
      AuthDbContext authDb,
      HttpContext httpContext,
      CancellationToken cancellationToken)
  {
    var currentUserId = CurrentUserContext.GetUserId(httpContext.User);
    if (currentUserId is null)
    {
      return Results.Unauthorized();
    }

    var tournament = await tournamentDb.Tournaments
        .AsNoTracking()
        .Include(t => t.Venue)
        .Include(t => t.Members)
        .Include(t => t.Disciplines)
            .ThenInclude(td => td.Discipline)
        .Include(t => t.Disciplines)
            .ThenInclude(td => td.Bouts)
                .ThenInclude(b => b.Rounds)
        .FirstOrDefaultAsync(t => t.Id == tournamentId, cancellationToken);

    if (tournament is null)
    {
      return Results.NotFound(new { message = "Tournament not found." });
    }

    var userIds = tournament.Members.Select(m => m.UserId).Distinct().ToArray();

    var usersById = await authDb.Users
        .AsNoTracking()
        .Where(u => userIds.Contains(u.Id))
        .Select(u => new { u.Id, u.FirstName, u.LastName, u.Email, u.Nickname })
        .ToDictionaryAsync(u => u.Id, cancellationToken);

    TournamentMemberResponse MapMember(TournamentMember member)
    {
      if (!usersById.TryGetValue(member.UserId, out var user))
      {
        return new TournamentMemberResponse(
            member.UserId,
            member.UserId.ToString(),
            string.Empty,
            null,
            member.Role.ToString());
      }

      var displayName = string.Join(" ", new[] { user.FirstName, user.LastName }.Where(s => !string.IsNullOrWhiteSpace(s))).Trim();
      if (string.IsNullOrWhiteSpace(displayName))
      {
        displayName = user.Email;
      }

      return new TournamentMemberResponse(
          user.Id,
          displayName,
          user.Email,
          user.Nickname,
          member.Role.ToString());
    }

    var organizers = tournament.Members
        .Where(m => m.Role == TournamentMemberRole.Organizer)
        .Select(MapMember)
        .OrderBy(m => m.DisplayName)
        .ToArray();

    var staff = tournament.Members
        .Where(m => m.Role == TournamentMemberRole.Staff)
        .Select(MapMember)
        .OrderBy(m => m.DisplayName)
        .ToArray();

    var participants = tournament.Members
        .Where(m => m.Role == TournamentMemberRole.Participant)
        .Select(MapMember)
        .OrderBy(m => m.DisplayName)
        .ToArray();

    var myRoles = tournament.Members
        .Where(m => m.UserId == currentUserId.Value)
        .Select(m => m.Role)
        .Distinct()
        .ToArray();

    var currentUserRole = myRoles.Contains(TournamentMemberRole.Organizer)
        ? TournamentMemberRole.Organizer.ToString()
        : myRoles.Contains(TournamentMemberRole.Staff)
            ? TournamentMemberRole.Staff.ToString()
            : myRoles.Contains(TournamentMemberRole.Participant)
                ? TournamentMemberRole.Participant.ToString()
                : null;

    var canManageStaff = myRoles.Contains(TournamentMemberRole.Organizer);
    var canScore = myRoles.Contains(TournamentMemberRole.Organizer) || myRoles.Contains(TournamentMemberRole.Staff);
    var hasJoined = myRoles.Length > 0;
    var nowUtc = DateTime.UtcNow;
    var canJoin = !hasJoined && nowUtc >= tournament.SignupStartDateUtc && nowUtc <= tournament.SignupEndDateUtc;

    var disciplines = tournament.Disciplines
        .OrderBy(td => td.Discipline.Name)
        .Select(td => new TournamentDisciplineDetailResponse(
            td.Id,
            td.DisciplineId,
            td.Discipline.Code,
            td.Discipline.Name,
            td.RoundCount,
            td.BoutIntervalMinutes,
            td.Bouts
                .OrderBy(b => b.ScheduledStartUtc)
                .Select(b => new TournamentBoutResponse(
                    b.Id,
                    b.ScheduledStartUtc,
                    b.Status.ToString(),
                    b.ParticipantAUserId,
                    b.ParticipantBUserId,
                    b.ParticipantATotalScore,
                    b.ParticipantBTotalScore,
                    b.WinnerUserId,
                    b.Rounds
                        .OrderBy(r => r.RoundNumber)
                        .Select(r => new TournamentRoundResponse(
                            r.RoundNumber,
                            r.ParticipantAScore,
                            r.ParticipantBScore))
                        .ToArray()))
                .ToArray()))
        .ToArray();

    var response = new TournamentDetailResponse(
        tournament.Id,
        tournament.Name,
        tournament.StartDateUtc,
        tournament.EndDateUtc,
        tournament.SignupStartDateUtc,
        tournament.SignupEndDateUtc,
        tournament.Venue.Name,
        tournament.Venue.Latitude,
        tournament.Venue.Longitude,
        tournament.Status.ToString(),
        currentUserRole,
        canJoin,
        canManageStaff,
        canScore,
        organizers,
        staff,
        participants,
        disciplines);

    return Results.Ok(response);
  }
}
