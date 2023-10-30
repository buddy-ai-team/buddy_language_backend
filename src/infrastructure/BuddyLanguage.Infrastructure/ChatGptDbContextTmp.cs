using Microsoft.EntityFrameworkCore;
using OpenAI.ChatGpt.Models;

namespace BuddyLanguage.Infrastructure;

internal class ChatGptDbContextTmp : DbContext
{
    public ChatGptDbContextTmp(DbContextOptions<ChatGptDbContextTmp> options)
        : base(options)
    {
    }

    public DbSet<Topic> Topics => this.Set<Topic>();

    public DbSet<PersistentChatMessage> Messages => this.Set<PersistentChatMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Topic>()
            .OwnsOne<ChatGPTConfig>(it => it.Config);
    }
}
