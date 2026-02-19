namespace TournamentPlanner.Data.Entities;

public class UserPermission
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string Permission { get; set; }
    public Guid? TournamentId { get; set; }
    public User User { get; set; } = null!;
}
