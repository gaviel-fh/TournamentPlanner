namespace TournamentPlanner.Data.Entities;

public class Venue
{
  public Guid Id { get; set; }
  public required string Name { get; set; }
  public string? AddressLine1 { get; set; }
  public string? City { get; set; }
  public string? Country { get; set; }
  public string? Notes { get; set; }

  public ICollection<Tournament> Tournaments { get; set; } = [];
}
