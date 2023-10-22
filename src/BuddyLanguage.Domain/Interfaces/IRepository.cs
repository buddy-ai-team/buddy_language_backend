namespace BuddyLanguage.Domain.Interfaces;

public interface IRepository<TEntity> where TEntity: class
{
    Task<TEntity> GetById(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> GetAll(CancellationToken cancellationToken = default);
    Task Add(TEntity entity, CancellationToken cancellationToken = default);
    Task Update(TEntity entity, CancellationToken cancellationToken = default);
    Task Delete(TEntity entity, CancellationToken cancellationToken = default);
}