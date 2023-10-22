namespace BuddyLanguage.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRoleRepository RoleRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}