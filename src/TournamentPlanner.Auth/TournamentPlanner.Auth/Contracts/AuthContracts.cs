namespace TournamentPlanner.Auth.Contracts;

public record RegisterRequest(string Email, string Password, string FirstName, string LastName, string? Nickname = null);
public record LoginRequest(string Email, string Password);
public record RefreshRequest(string RefreshToken);
public record AuthResponse(string AccessToken, string RefreshToken);
