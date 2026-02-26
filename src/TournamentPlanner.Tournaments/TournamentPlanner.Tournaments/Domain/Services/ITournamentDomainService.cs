using TournamentPlanner.Tournaments.Domain.Models;

namespace TournamentPlanner.Tournaments.Domain.Services;

public interface ITournamentDomainService
{
  IReadOnlyCollection<string> ValidateTournamentCreation(TournamentCreationModel model);
  IReadOnlyCollection<string> ValidateScheduleGeneration(DateTime nowUtc, DateTime signupEndDateUtc, int participantCount);
  BoutScoreState ApplyRoundScore(BoutScoreState state, Guid awardedToUserId, int points);
}
