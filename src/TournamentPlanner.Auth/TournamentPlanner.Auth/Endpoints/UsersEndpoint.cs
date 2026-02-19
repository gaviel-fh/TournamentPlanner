using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using TournamentPlanner.Auth.Contracts;
using TournamentPlanner.Data;

namespace TournamentPlanner.Auth.Endpoints;

public static class UsersEndpoint
{
  public static RouteGroupBuilder MapUsers(this RouteGroupBuilder group)
  {
    group.MapGet("/users", Handle).RequireAuthorization();
    return group;
  }

  private static async Task<IResult> Handle(AuthDbContext db, CancellationToken cancellationToken)
  {
    var users = await db.Users
        .AsNoTracking()
        .OrderBy(u => u.FirstName)
        .ThenBy(u => u.LastName)
        .ThenBy(u => u.Email)
        .Select(u => new UserLookupResponse(
            u.Id,
            u.Email,
            u.FirstName,
            u.LastName,
            u.Nickname))
        .ToListAsync(cancellationToken);

    return Results.Ok(users);
  }
}
