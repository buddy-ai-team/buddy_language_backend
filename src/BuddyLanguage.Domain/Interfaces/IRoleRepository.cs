using BuddyLanguage.Domain.Entities;

namespace BuddyLanguage.Domain.Interfaces;

public interface IRoleRepository : IRepository<Role>
{
    Task<IReadOnlyList<Role>> GetAll(CancellationToken cancellationToken);
}
