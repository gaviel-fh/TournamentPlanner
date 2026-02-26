using System.Security.Claims;

namespace TournamentPlanner.Tournaments.Security;

public static class CurrentUserContext
{
  public static Guid? GetUserId(ClaimsPrincipal user)
  {
    var raw = user.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? user.FindFirstValue("sub");

    return Guid.TryParse(raw, out var id) ? id : null;
  }
}
