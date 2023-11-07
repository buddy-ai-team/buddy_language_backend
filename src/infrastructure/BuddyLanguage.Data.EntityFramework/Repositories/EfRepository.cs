using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuddyLanguage.Data.EntityFramework.Repositories;

public class EfRepository<TEntity> : IRepository<TEntity>
    where TEntity : class, IEntity
{
    protected readonly AppDbContext DbContext;

    public EfRepository(AppDbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    protected DbSet<TEntity> Entities => DbContext.Set<TEntity>();

    public virtual async Task<TEntity> GetById(Guid id, CancellationToken cancellationToken)
        => await Entities.FirstAsync(it => it.Id == id, cancellationToken);

    public virtual async Task<IReadOnlyList<TEntity>> GetAll(CancellationToken cancellationToken)
            => await Entities.ToListAsync(cancellationToken);

    public virtual async Task Add(TEntity entity, CancellationToken cancellationToken)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        await Entities.AddAsync(entity, cancellationToken);
    }

    public virtual Task Update(TEntity entity, CancellationToken cancellationToken)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        DbContext.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public virtual Task Delete(TEntity entity, CancellationToken cancellationToken)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        Entities.Remove(entity);
        return Task.CompletedTask;
    }
}
