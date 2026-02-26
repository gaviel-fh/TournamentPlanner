namespace TournamentPlanner.Data.Entities;

public class Bout
{
  public Guid Id { get; set; }
  public Guid TournamentDisciplineId { get; set; }
  public Guid ParticipantAUserId { get; set; }
  public Guid ParticipantBUserId { get; set; }
  public DateTime ScheduledStartUtc { get; set; }
  public BoutStatus Status { get; set; } = BoutStatus.Scheduled;
  public int ParticipantATotalScore { get; set; }
  public int ParticipantBTotalScore { get; set; }
  public Guid? WinnerUserId { get; set; }

  public TournamentDiscipline TournamentDiscipline { get; set; } = null!;
  public ICollection<BoutRound> Rounds { get; set; } = [];
}
