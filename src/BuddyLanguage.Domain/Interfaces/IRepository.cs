﻿namespace BuddyLanguage.Domain.Interfaces;

public interface IRepository<TEntity>
    where TEntity : class
{
    Task<TEntity> GetById(Guid id, CancellationToken cancellationToken);

    Task Add(TEntity entity, CancellationToken cancellationToken);

    Task Update(TEntity entity, CancellationToken cancellationToken);

    Task Delete(TEntity entity, CancellationToken cancellationToken);
}
