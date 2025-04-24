using Microsoft.EntityFrameworkCore;
using EpicShowdown.API.Models.Entities;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}