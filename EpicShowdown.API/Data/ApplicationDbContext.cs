using Microsoft.EntityFrameworkCore;
using EpicShowdown.API.Models.Entities;
using EpicShowdown.API.Models;

namespace EpicShowdown.API.Data;

public class ApplicationDbContext : DbContext
{
    // dotnet ef migrations add InitialCreate
    // dotnet ef database update
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Contest> Contests { get; set; }
    public DbSet<Contestant> Contestants { get; set; }
    public DbSet<ContestantField> ContestantFields { get; set; }
    public DbSet<Gift> Gifts { get; set; }
    public DbSet<ContestGift> ContestGifts { get; set; }
    public DbSet<ContestantGift> ContestantGifts { get; set; }
    public DbSet<PassKey> PassKeys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Contest>()
            .HasMany(c => c.Contestants)
            .WithOne(c => c.Contest)
            .HasForeignKey(c => c.ContestId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Contest>()
            .HasMany(c => c.Fields)
            .WithOne(f => f.Contest)
            .HasForeignKey(f => f.ContestId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Contest>()
            .HasMany(c => c.ContestGifts)
            .WithOne(cg => cg.Contest)
            .HasForeignKey(cg => cg.ContestId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Gift>()
            .HasMany(g => g.ContestGifts)
            .WithOne(cg => cg.Gift)
            .HasForeignKey(cg => cg.GiftId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PassKey>()
            .HasIndex(p => p.CredentialId)
            .IsUnique();
    }
}