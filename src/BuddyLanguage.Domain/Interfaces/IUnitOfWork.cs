namespace BuddyLanguage.Domain.Interfaces;

public interface IUnitOfWork
{
    IRoleRepository RoleRepository { get; }
    IWordEntityRepository WordEntityRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}