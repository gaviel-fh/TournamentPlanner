namespace TournamentPlanner.Data.Entities;

public class TournamentUserProfile
{
  public Guid UserId { get; set; }
  public string? DisplayName { get; set; }
  public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}