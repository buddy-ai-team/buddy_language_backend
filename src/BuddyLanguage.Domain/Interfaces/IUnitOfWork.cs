namespace BuddyLanguage.Domain.Interfaces;

public interface IUnitOfWork
{
    IRoleRepository RoleRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}