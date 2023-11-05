using BuddyLanguage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuddyLanguage.Data.EntityFramework;

public class AppDbContext : DbContext
{ 
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<WordEntity> WordEntities => Set<WordEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Define the relationship between User and WordEntity
        modelBuilder.Entity<User>()
            .HasMany(user => user.WordEntities)
            .WithOne(wordEntity => wordEntity.User)
            .HasForeignKey(wordEntity => wordEntity.UserId);

        // Configure UserPreferences as an owned type
        modelBuilder.Entity<User>()
            .OwnsOne(user => user.UserPreferences, preferences =>
            {
                preferences.Property(p => p.NativeLanguage).HasConversion<string>();
                preferences.Property(p => p.LearnedLanguage).HasConversion<string>();
                preferences.Property(p => p.SelectedVoice).HasConversion<string>();
            });
    }
}
