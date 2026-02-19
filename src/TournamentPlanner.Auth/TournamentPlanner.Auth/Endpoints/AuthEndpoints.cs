using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace TournamentPlanner.Auth.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth").WithTags("Authentication");

        group.MapRegister();
        group.MapLogin();
        group.MapRefresh();
        group.MapLogout();
        group.MapUsers();

        return app;
    }
}
