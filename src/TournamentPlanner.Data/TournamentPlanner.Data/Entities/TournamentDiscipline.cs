namespace TournamentPlanner.Data.Entities;

public class TournamentDiscipline
{
  public Guid Id { get; set; }
  public Guid TournamentId { get; set; }
  public Guid DisciplineId { get; set; }
  public int RoundCount { get; set; } = 3;
  public int BoutIntervalMinutes { get; set; } = 30;

  public Tournament Tournament { get; set; } = null!;
  public Discipline Discipline { get; set; } = null!;
  public ICollection<Bout> Bouts { get; set; } = [];
}
