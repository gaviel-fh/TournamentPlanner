using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TournamentPlanner.Auth.Authorization;
using TournamentPlanner.Auth.Configuration;
using TournamentPlanner.Auth.Services;

namespace TournamentPlanner.Auth.Extensions;

public static class AuthServiceCollectionExtensions
{
    public static IServiceCollection AddTournamentAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection(JwtSettings.SectionName);
        services.Configure<JwtSettings>(jwtSection);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var settings = jwtSection.Get<JwtSettings>()!;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = settings.Issuer,
                    ValidAudience = settings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(settings.Secret))
                };
            });

        services.AddAuthorization(options =>
        {
            foreach (var permission in GetAllPermissions())
            {
                options.AddPolicy(permission, policy =>
                    policy.Requirements.Add(new HasPermissionRequirement(permission)));
            }
        });

        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }

    private static IEnumerable<string> GetAllPermissions()
    {
        return typeof(Permissions)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.IsLiteral && !f.IsInitOnly)
            .Select(f => (string)f.GetRawConstantValue()!);
    }
}
