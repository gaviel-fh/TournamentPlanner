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

public static class LoginEndpoint
{
    public static RouteGroupBuilder MapLogin(this RouteGroupBuilder group)
    {
        group.MapPost("/login", Handle).AllowAnonymous();
        return group;
    }

    private static async Task<IResult> Handle(
        LoginRequest request,
        AppDbContext db,
        ITokenService tokenService,
        IOptions<JwtSettings> jwtSettings)
    {
        var user = await db.Users
            .Include(u => u.Permissions)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
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
