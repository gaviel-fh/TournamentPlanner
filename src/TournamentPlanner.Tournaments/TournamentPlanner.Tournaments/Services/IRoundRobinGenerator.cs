namespace TournamentPlanner.Tournaments.Services;

public interface IRoundRobinGenerator
{
  IReadOnlyCollection<(Guid ParticipantA, Guid ParticipantB)> GeneratePairings(
      IReadOnlyCollection<Guid> participantIds);
}
