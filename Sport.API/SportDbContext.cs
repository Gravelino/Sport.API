using Microsoft.EntityFrameworkCore;
using Sport.API.Entities;

namespace Sport.API;

public class SportDbContext : DbContext
{
    public SportDbContext(DbContextOptions<SportDbContext> options) : base(options)
    {
            
    }

    public DbSet<Participant> Participants { get; set; }
    public DbSet<Competition> Competitions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}