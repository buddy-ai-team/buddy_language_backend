using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuddyLanguage.Data.EntityFramework.Repositories;

public class RoleRepositoryEf : EfRepository<Role>, IRoleRepository
{
    public RoleRepositoryEf(AppDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<Role>> GetAll(CancellationToken cancellationToken)
        => await Entities.ToListAsync(cancellationToken);

    public async Task<Role?> FindRoleByRoleType(
        RoleType roleType, CancellationToken cancellationToken)
    {
       return await Entities.SingleOrDefaultAsync(e => e.RoleType == roleType, cancellationToken);
    } 
}
