using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;

namespace BuddyLanguage.Domain.Interfaces;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> FindRoleByRoleType(RoleType roleType, CancellationToken cancellationToken);
}
