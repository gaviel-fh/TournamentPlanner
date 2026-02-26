namespace TournamentPlanner.Data.Entities;

public class Tournament
{
  public Guid Id { get; set; }
  public required string Name { get; set; }
  public Guid VenueId { get; set; }
  public DateTime StartDateUtc { get; set; }
  public DateTime EndDateUtc { get; set; }
  public DateTime SignupStartDateUtc { get; set; }
  public DateTime SignupEndDateUtc { get; set; }
  public TournamentStatus Status { get; set; } = TournamentStatus.Draft;
  public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

  public Venue Venue { get; set; } = null!;
  public ICollection<TournamentDiscipline> Disciplines { get; set; } = [];
  public ICollection<TournamentMember> Members { get; set; } = [];
}
