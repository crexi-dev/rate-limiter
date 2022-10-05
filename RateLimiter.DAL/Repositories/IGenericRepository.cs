using RateLimiter.DAL.Entities.Interfaces;

namespace RateLimiter.DAL.Repositories;

public interface IGenericRepository<T> where T : Entity
{
    Task<T> Create(T entity, CancellationToken token);
    Task<T?> Read(Guid id, CancellationToken token);
    Task<T> Update(T entity, CancellationToken token);
    Task Delete(T entity, CancellationToken token);
    Task Delete(Guid id, CancellationToken token);
    Task<T?> CreateOrUpdate(T entity, CancellationToken token);
}