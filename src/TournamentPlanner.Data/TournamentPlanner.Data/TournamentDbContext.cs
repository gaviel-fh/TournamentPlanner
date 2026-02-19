using Microsoft.EntityFrameworkCore;
using TournamentPlanner.Data.Entities;

namespace TournamentPlanner.Data;

public class TournamentDbContext(DbContextOptions<TournamentDbContext> options) : DbContext(options)
{
  public DbSet<Venue> Venues => Set<Venue>();
  public DbSet<Tournament> Tournaments => Set<Tournament>();
  public DbSet<Discipline> Disciplines => Set<Discipline>();
  public DbSet<TournamentDiscipline> TournamentDisciplines => Set<TournamentDiscipline>();
  public DbSet<TournamentMember> TournamentMembers => Set<TournamentMember>();
  public DbSet<Bout> Bouts => Set<Bout>();
  public DbSet<BoutRound> BoutRounds => Set<BoutRound>();
  public DbSet<ScoreEvent> ScoreEvents => Set<ScoreEvent>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Venue>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Name).HasMaxLength(200);
      entity.Property(e => e.AddressLine1).HasMaxLength(300);
      entity.Property(e => e.City).HasMaxLength(120);
      entity.Property(e => e.Country).HasMaxLength(120);
    });

    modelBuilder.Entity<Tournament>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Name).HasMaxLength(200);
      entity.HasIndex(e => new { e.StartDateUtc, e.EndDateUtc });
      entity.HasOne(e => e.Venue)
              .WithMany(v => v.Tournaments)
              .HasForeignKey(e => e.VenueId)
              .OnDelete(DeleteBehavior.Restrict);
    });

    modelBuilder.Entity<Discipline>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Code).HasMaxLength(40);
      entity.Property(e => e.Name).HasMaxLength(120);
      entity.HasIndex(e => e.Code).IsUnique();
    });

    modelBuilder.Entity<TournamentDiscipline>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.HasIndex(e => new { e.TournamentId, e.DisciplineId }).IsUnique();
      entity.HasOne(e => e.Tournament)
              .WithMany(t => t.Disciplines)
              .HasForeignKey(e => e.TournamentId)
              .OnDelete(DeleteBehavior.Cascade);
      entity.HasOne(e => e.Discipline)
              .WithMany(d => d.TournamentDisciplines)
              .HasForeignKey(e => e.DisciplineId)
              .OnDelete(DeleteBehavior.Restrict);
    });

    modelBuilder.Entity<TournamentMember>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.HasIndex(e => new { e.TournamentId, e.UserId, e.Role }).IsUnique();
      entity.HasOne(e => e.Tournament)
              .WithMany(t => t.Members)
              .HasForeignKey(e => e.TournamentId)
              .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<Bout>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.HasIndex(e => new { e.TournamentDisciplineId, e.ScheduledStartUtc });
      entity.HasOne(e => e.TournamentDiscipline)
              .WithMany(d => d.Bouts)
              .HasForeignKey(e => e.TournamentDisciplineId)
              .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<BoutRound>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.HasIndex(e => new { e.BoutId, e.RoundNumber }).IsUnique();
      entity.HasOne(e => e.Bout)
              .WithMany(b => b.Rounds)
              .HasForeignKey(e => e.BoutId)
              .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<ScoreEvent>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Reason).HasMaxLength(300);
      entity.HasIndex(e => new { e.BoutRoundId, e.OccurredAtUtc });
      entity.HasOne(e => e.BoutRound)
              .WithMany(r => r.ScoreEvents)
              .HasForeignKey(e => e.BoutRoundId)
              .OnDelete(DeleteBehavior.Cascade);
    });
  }
}
