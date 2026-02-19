using Microsoft.AspNetCore.Authorization;

namespace TournamentPlanner.Auth.Authorization;

public class HasPermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
