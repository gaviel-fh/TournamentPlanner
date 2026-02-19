namespace TournamentPlanner.Data.Entities;

public class Discipline
{
  public Guid Id { get; set; }
  public required string Code { get; set; }
  public required string Name { get; set; }

  public ICollection<TournamentDiscipline> TournamentDisciplines { get; set; } = [];
}
