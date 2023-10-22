using BuddyLanguage.Domain.Entities;

namespace BuddyLanguage.Domain.Interfaces;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByName(string name, CancellationToken cancellationToken = default);
}