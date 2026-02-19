using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using TournamentPlanner.Data;

namespace TournamentPlanner.Auth.Endpoints;

public static class LogoutEndpoint
{
    public static RouteGroupBuilder MapLogout(this RouteGroupBuilder group)
    {
        group.MapPost("/logout", Handle).RequireAuthorization();
        return group;
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        AuthDbContext db)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null || !Guid.TryParse(userId, out var id))
            return Results.Unauthorized();

        var dbUser = await db.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (dbUser is null)
            return Results.Unauthorized();

        dbUser.RefreshToken = null;
        dbUser.RefreshTokenExpiryTime = null;
        await db.SaveChangesAsync();

        return Results.Ok(new { Message = "Logged out successfully." });
    }
}
