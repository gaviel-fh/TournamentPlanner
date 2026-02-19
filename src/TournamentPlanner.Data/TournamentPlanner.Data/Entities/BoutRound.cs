namespace TournamentPlanner.Data.Entities;

public class BoutRound
{
  public Guid Id { get; set; }
  public Guid BoutId { get; set; }
  public int RoundNumber { get; set; }
  public int ParticipantAScore { get; set; }
  public int ParticipantBScore { get; set; }

  public Bout Bout { get; set; } = null!;
  public ICollection<ScoreEvent> ScoreEvents { get; set; } = [];
}
