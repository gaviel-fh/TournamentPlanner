using TournamentPlanner.Tournaments.Domain.Models;

namespace TournamentPlanner.Tournaments.Domain.Services;

public class TournamentDomainService : ITournamentDomainService
{
  public IReadOnlyCollection<string> ValidateTournamentCreation(TournamentCreationModel model)
  {
    var errors = new List<string>();

    if (string.IsNullOrWhiteSpace(model.Name))
    {
      errors.Add("Tournament name is required.");
    }

    if (string.IsNullOrWhiteSpace(model.Location.VenueName))
    {
      errors.Add("Venue name is required.");
    }

    if (model.EndDateUtc < model.StartDateUtc)
    {
      errors.Add("End date must be after start date.");
    }

    if (model.SignupEndDateUtc < model.SignupStartDateUtc)
    {
      errors.Add("Signup end date must be after signup start date.");
    }

    if (model.SignupStartDateUtc < model.StartDateUtc || model.SignupEndDateUtc > model.EndDateUtc)
    {
      errors.Add("Signup window must be within tournament start/end dates.");
    }

    if (model.Location.Latitude is < -90 or > 90)
    {
      errors.Add("Latitude must be between -90 and 90.");
    }

    if (model.Location.Longitude is < -180 or > 180)
    {
      errors.Add("Longitude must be between -180 and 180.");
    }

    if (model.Disciplines.Count == 0)
    {
      errors.Add("At least one discipline is required.");
    }

    foreach (var discipline in model.Disciplines)
    {
      if (string.IsNullOrWhiteSpace(discipline.Code) || string.IsNullOrWhiteSpace(discipline.Name))
      {
        errors.Add("Each discipline requires code and name.");
      }

      if (discipline.RoundCount <= 0)
      {
        errors.Add("Round count must be greater than zero.");
      }
    }

    return errors;
  }

  public IReadOnlyCollection<string> ValidateScheduleGeneration(DateTime nowUtc, DateTime signupEndDateUtc, int participantCount)
  {
    var errors = new List<string>();

    if (participantCount < 2)
    {
      errors.Add("At least two participants are required.");
    }

    if (nowUtc < signupEndDateUtc)
    {
      errors.Add("Pairings can only be generated after the signup window ends.");
    }

    return errors;
  }

  public BoutScoreState ApplyRoundScore(BoutScoreState state, Guid awardedToUserId, int points)
  {
    var participantATotal = state.ParticipantATotalScore;
    var participantBTotal = state.ParticipantBTotalScore;

    if (awardedToUserId == state.ParticipantAUserId)
    {
      participantATotal += points;
    }
    else if (awardedToUserId == state.ParticipantBUserId)
    {
      participantBTotal += points;
    }
    else
    {
      throw new InvalidOperationException("Awarded user is not a participant in this bout.");
    }

    Guid? winnerUserId = participantATotal == participantBTotal
        ? null
        : participantATotal > participantBTotal
            ? state.ParticipantAUserId
            : state.ParticipantBUserId;

    return state with
    {
      ParticipantATotalScore = participantATotal,
      ParticipantBTotalScore = participantBTotal,
      WinnerUserId = winnerUserId
    };
  }
}
