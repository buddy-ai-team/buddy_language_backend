using BuddyLanguage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace BuddyLanguage.Data.EntityFramework;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<WordEntity> WordEntities => Set<WordEntity>();

    public AppDbContext(
        DbContextOptions<AppDbContext> options) :
        base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Define the relationship between User and WordEntity
        modelBuilder.Entity<User>()
            .HasMany(user => user.WordEntities)
            .WithOne(wordEntity => wordEntity.User)
            .HasForeignKey(wordEntity => wordEntity.UserId);
    }
}