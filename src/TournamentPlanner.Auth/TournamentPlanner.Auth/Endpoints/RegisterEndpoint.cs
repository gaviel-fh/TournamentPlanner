using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TournamentPlanner.Auth.Configuration;
using TournamentPlanner.Auth.Contracts;
using TournamentPlanner.Auth.Services;
using TournamentPlanner.Data;
using TournamentPlanner.Data.Entities;

namespace TournamentPlanner.Auth.Endpoints;

public static class RegisterEndpoint
{
    public static RouteGroupBuilder MapRegister(this RouteGroupBuilder group)
    {
        group.MapPost("/register", Handle).AllowAnonymous();
        return group;
    }

    private static async Task<IResult> Handle(
        RegisterRequest request,
        AppDbContext db,
        ITokenService tokenService,
        IOptions<JwtSettings> jwtSettings)
    {
        if (await db.Users.AnyAsync(u => u.Email == request.Email))
            return Results.Conflict(new { Message = "Email already registered." });

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Nickname = request.Nickname,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        var accessToken = tokenService.GenerateAccessToken(user, []);
        var refreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(jwtSettings.Value.RefreshTokenExpirationDays);

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return Results.Created($"/auth/{user.Id}", new AuthResponse(accessToken, refreshToken));
    }
}
