using BadmintonClubAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BadmintonClubAPI.Data;

public class BadmintonDbContext : DbContext
{
    public BadmintonDbContext(DbContextOptions<BadmintonDbContext> options) : base(options)
    {
    }

    public DbSet<Player> Players => Set<Player>();
    public DbSet<Court> Courts => Set<Court>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<League> Leagues => Set<League>();
    public DbSet<MatchParticipant> MatchParticipants => Set<MatchParticipant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<MatchParticipant>().HasKey(mp => new { mp.MatchId, mp.PlayerId });
    }
}