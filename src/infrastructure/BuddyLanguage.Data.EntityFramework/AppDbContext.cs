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

        // Define the one-to-one relationship between User and Role
        modelBuilder.Entity<User>()
            .HasOne(u => u.UserPreferences.AssistantRole)
            .WithOne()
            .HasForeignKey<User>(u => u.UserPreferences.AssistantRoleId)
            .IsRequired(false);

        // Configure UserPreferences as an owned type
        modelBuilder.Entity<User>()
            .OwnsOne(user => user.UserPreferences, preferences =>
            {
                preferences.Property(p => p.NativeLanguage).HasConversion<string>();
                preferences.Property(p => p.TargetLanguage).HasConversion<string>();
                preferences.Property(p => p.SelectedVoice).HasConversion<string>();
                preferences.Property(p => p.SelectedSpeed).HasConversion<string>();
            });
    }
}
