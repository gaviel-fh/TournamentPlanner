namespace TournamentPlanner.Tournaments.Services;

public class RoundRobinGenerator : IRoundRobinGenerator
{
  public IReadOnlyCollection<(Guid ParticipantA, Guid ParticipantB)> GeneratePairings(
      IReadOnlyCollection<Guid> participantIds)
  {
    var participants = participantIds.Distinct().ToList();
    if (participants.Count < 2)
    {
      return [];
    }

    var hasBye = participants.Count % 2 == 1;
    if (hasBye)
    {
      participants.Add(Guid.Empty);
    }

    var pairings = new List<(Guid ParticipantA, Guid ParticipantB)>();
    var totalRounds = participants.Count - 1;
    var participantsPerRound = participants.Count / 2;

    for (var roundIndex = 0; roundIndex < totalRounds; roundIndex++)
    {
      for (var i = 0; i < participantsPerRound; i++)
      {
        var participantA = participants[i];
        var participantB = participants[^(i + 1)];

        if (participantA != Guid.Empty && participantB != Guid.Empty)
        {
          pairings.Add((participantA, participantB));
        }
      }

      var anchor = participants[0];
      var rotating = participants.Skip(1).ToList();
      var last = rotating[^1];
      rotating.RemoveAt(rotating.Count - 1);
      rotating.Insert(0, last);

      participants.Clear();
      participants.Add(anchor);
      participants.AddRange(rotating);
    }

    return pairings;
  }
}
