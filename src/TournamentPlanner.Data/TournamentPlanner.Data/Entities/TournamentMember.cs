namespace TournamentPlanner.Data.Entities;

public class TournamentMember
{
  public Guid Id { get; set; }
  public Guid TournamentId { get; set; }
  public Guid UserId { get; set; }
  public TournamentMemberRole Role { get; set; }

  public Tournament Tournament { get; set; } = null!;
}
