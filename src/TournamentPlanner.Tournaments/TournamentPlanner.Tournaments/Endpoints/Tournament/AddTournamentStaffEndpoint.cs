using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TournamentPlanner.Data;
using TournamentPlanner.Data.Entities;
using TournamentPlanner.Tournaments.Contracts;
using TournamentPlanner.Tournaments.Security;

namespace TournamentPlanner.Tournaments.Endpoints.Tournament;

public static class AddTournamentStaffEndpoint
{
  public static async Task<IResult> Handle(
      Guid tournamentId,
      AddStaffRequest request,
      TournamentDbContext db,
      AuthDbContext authDb,
      HttpContext httpContext,
      CancellationToken cancellationToken)
  {
    var currentUserId = CurrentUserContext.GetUserId(httpContext.User);
    if (currentUserId is null)
    {
      return Results.Unauthorized();
    }

    var isOrganizer = await db.TournamentMembers.AnyAsync(
        m => m.TournamentId == tournamentId
             && m.UserId == currentUserId.Value
             && m.Role == TournamentMemberRole.Organizer,
        cancellationToken);

    if (!isOrganizer)
    {
      return Results.Forbid();
    }

    if (request.UserId == Guid.Empty)
    {
      return Results.BadRequest(new { message = "UserId is required." });
    }

    var userExists = await authDb.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
    if (!userExists)
    {
      return Results.BadRequest(new { message = "User does not exist." });
    }

    var tournamentExists = await db.Tournaments.AnyAsync(t => t.Id == tournamentId, cancellationToken);
    if (!tournamentExists)
    {
      return Results.NotFound(new { message = "Tournament not found." });
    }

    var existingStaff = await db.TournamentMembers.AnyAsync(
        m => m.TournamentId == tournamentId
             && m.UserId == request.UserId
             && m.Role == TournamentMemberRole.Staff,
        cancellationToken);

    if (existingStaff)
    {
      return Results.Ok(new { added = false, message = "User is already staff." });
    }

    db.TournamentMembers.Add(new TournamentMember
    {
      Id = Guid.NewGuid(),
      TournamentId = tournamentId,
      UserId = request.UserId,
      Role = TournamentMemberRole.Staff
    });

    var hasProfile = await db.TournamentUserProfiles.AnyAsync(p => p.UserId == request.UserId, cancellationToken);
    if (!hasProfile)
    {
      db.TournamentUserProfiles.Add(new TournamentUserProfile { UserId = request.UserId });
    }

    await db.SaveChangesAsync(cancellationToken);

    return Results.Ok(new { added = true });
  }
}
