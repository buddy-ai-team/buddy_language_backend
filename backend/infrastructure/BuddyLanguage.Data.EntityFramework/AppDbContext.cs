using Microsoft.EntityFrameworkCore;

namespace BuddyLanguage.Data.EntityFramework;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options) :
        base(options)
    {
    }
}