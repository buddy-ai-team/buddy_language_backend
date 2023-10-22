using BuddyLanguage.Domain.Interfaces;

namespace BuddyLanguage.Data.EntityFramework.Repositories;

public class UnitOfWorkEf : IUnitOfWork, IAsyncDisposable
{
    public IRoleRepository RoleRepository { get; }
    
    private readonly AppDbContext _dbContext;
    
    public UnitOfWorkEf(
        AppDbContext dbContext,
        IRoleRepository roleRepository)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            RoleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        }
    
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);
    public void Dispose() => _dbContext.Dispose();
    public ValueTask DisposeAsync() => _dbContext.DisposeAsync();
}