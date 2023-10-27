using Microsoft.EntityFrameworkCore;

namespace BuddyLanguage.Data.EntityFramework;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options) :
        base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //Add WordEntity To Account External Key Once Account Is Implemented
    }
}