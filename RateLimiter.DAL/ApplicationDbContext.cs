using Microsoft.EntityFrameworkCore;
using RateLimiter.DAL.Entities;

namespace RateLimiter.DAL;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Token> Tokens { get; set; }
    public DbSet<SetByCountPerTime> SetsByCountPerTime { get; set; }
    public DbSet<SetByLastCall> SetsByLastCall { get; set; }
    public DbSet<History> Histories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Token>().HasIndex(x => x.Id);
        modelBuilder.Entity<SetByLastCall>().HasMany<Token>().WithOne().HasForeignKey(x => x.SetId);
        modelBuilder.Entity<SetByCountPerTime>().HasMany<Token>().WithOne().HasForeignKey(x => x.SetId);
        modelBuilder.Entity<History>().HasOne(x => x.Token).WithOne(x => x.History)
            .HasForeignKey<History>(x => x.TokenId);
    }
}