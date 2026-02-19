using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TournamentPlanner.Data;
using TournamentPlanner.Data.Entities;
using TournamentPlanner.Tournaments.Contracts;

namespace TournamentPlanner.Tournaments.Endpoints.Tournament;

public static class GetAllTournamentsEndpoint
{
  public static async Task<IResult> Handle(TournamentDbContext db)
  {
    var tournaments = await db.Tournaments
        .AsNoTracking()
        .Include(t => t.Venue)
        .Include(t => t.Disciplines)
        .Include(t => t.Members)
        .OrderByDescending(t => t.StartDateUtc)
        .Select(t => new TournamentSummaryResponse(
            t.Id,
            t.Name,
            t.StartDateUtc,
            t.EndDateUtc,
            t.Venue.Name,
            t.Disciplines.Count,
            t.Members.Count(m => m.Role == TournamentMemberRole.Participant),
            t.Status.ToString()))
        .ToListAsync();

    return Results.Ok(tournaments);
  }
}
