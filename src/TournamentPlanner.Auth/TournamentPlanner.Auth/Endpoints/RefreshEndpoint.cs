using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TournamentPlanner.Auth.Configuration;
using TournamentPlanner.Auth.Contracts;
using TournamentPlanner.Auth.Services;
using TournamentPlanner.Data;

namespace TournamentPlanner.Auth.Endpoints;

public static class RefreshEndpoint
{
    public static RouteGroupBuilder MapRefresh(this RouteGroupBuilder group)
    {
        group.MapPost("/refresh", Handle).AllowAnonymous();
        return group;
    }

    private static async Task<IResult> Handle(
        RefreshRequest request,
        AuthDbContext db,
        ITokenService tokenService,
        IOptions<JwtSettings> jwtSettings)
    {
        var user = await db.Users
            .Include(u => u.Permissions)
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

        if (user is null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            return Results.Unauthorized();

        var permissions = user.Permissions.Select(p => p.Permission).ToList();
        var accessToken = tokenService.GenerateAccessToken(user, permissions);
        var refreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(jwtSettings.Value.RefreshTokenExpirationDays);
        await db.SaveChangesAsync();

        return Results.Ok(new AuthResponse(accessToken, refreshToken));
    }
}
