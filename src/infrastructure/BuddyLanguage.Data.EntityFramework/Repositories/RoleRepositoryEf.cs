using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuddyLanguage.Data.EntityFramework.Repositories;

public class RoleRepositoryEf : EfRepository<Role>, IRoleRepository
{
    public RoleRepositoryEf(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Role?> GetByName(string name, CancellationToken cancellationToken = default)
    {
        if (name == null) 
            throw new ArgumentNullException(nameof(name));
        return await Entities.SingleOrDefaultAsync(e => e.Name == name, cancellationToken);
    }
}