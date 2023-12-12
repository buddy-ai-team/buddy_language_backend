namespace BuddyLanguage.Domain.Interfaces;

public interface IUnitOfWork
{
    IRoleRepository RoleRepository { get; }

    IWordEntityRepository WordEntityRepository { get; }

    IUserRepository UserRepository { get; }

    IMessageRepository MessageRepository { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
