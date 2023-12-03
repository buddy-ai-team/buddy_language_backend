using BuddyLanguage.Domain.Entities;
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
}
