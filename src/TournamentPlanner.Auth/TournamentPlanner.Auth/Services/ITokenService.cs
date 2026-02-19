using TournamentPlanner.Data.Entities;

namespace TournamentPlanner.Auth.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user, IEnumerable<string> permissions);
    string GenerateRefreshToken();
}
