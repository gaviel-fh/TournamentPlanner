namespace TournamentPlanner.Data.Entities;

public class ScoreEvent
{
  public Guid Id { get; set; }
  public Guid BoutRoundId { get; set; }
  public Guid AwardedToUserId { get; set; }
  public Guid AwardedByUserId { get; set; }
  public int Points { get; set; }
  public string? Reason { get; set; }
  public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;

  public BoutRound BoutRound { get; set; } = null!;
}
