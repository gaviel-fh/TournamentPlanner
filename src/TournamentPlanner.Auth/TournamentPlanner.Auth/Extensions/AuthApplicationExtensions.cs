using Microsoft.AspNetCore.Builder;
using TournamentPlanner.Auth.Endpoints;

namespace TournamentPlanner.Auth.Extensions;

public static class AuthApplicationExtensions
{
    public static WebApplication UseTournamentAuth(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapAuthEndpoints();

        return app;
    }
}
