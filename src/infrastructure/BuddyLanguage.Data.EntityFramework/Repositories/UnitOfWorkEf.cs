using BuddyLanguage.Domain.Interfaces;

namespace BuddyLanguage.Data.EntityFramework.Repositories;

public class UnitOfWorkEf : IUnitOfWork
{
    private readonly AppDbContext _dbContext;

    public UnitOfWorkEf(
        AppDbContext dbContext,
        IRoleRepository roleRepository,
        IWordEntityRepository wordRepository,
        IUserRepository userRepository,
        IMessageRepository messageRepository)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        RoleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        WordEntityRepository = wordRepository ?? throw new ArgumentNullException(nameof(wordRepository));
        UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        MessageRepository = messageRepository ?? throw new ArgumentNullException(nameof(messageRepository));
    }

    public IRoleRepository RoleRepository { get; }

    public IWordEntityRepository WordEntityRepository { get; }

    public IUserRepository UserRepository { get; }

    public IMessageRepository MessageRepository { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
