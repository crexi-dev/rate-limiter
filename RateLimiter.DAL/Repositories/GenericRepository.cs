using Microsoft.EntityFrameworkCore;
using RateLimiter.DAL.Entities.Interfaces;

namespace RateLimiter.DAL.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : Entity
{
    protected readonly ApplicationDbContext _context;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<T> Create(T entity, CancellationToken token)
    {
        await _context.Set<T>().AddAsync(entity, token);
        await _context.SaveChangesAsync(token);
        return entity;
    }

    public Task<T?> Read(Guid id, CancellationToken token)
    {
        return _context.Set<T>().AsNoTracking().AsQueryable()
            .SingleOrDefaultAsync(x => x.Id == id, token);
    }

    public async Task<T> Update(T entity, CancellationToken token)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync(token);
        return entity;
    }

    public Task Delete(T entity, CancellationToken token)
    {
        _context.Set<T>().Remove(entity);
        return _context.SaveChangesAsync(token);
    }

    public async Task Delete(Guid id, CancellationToken token)
    {
        var entity = await _context.Set<T>().AsQueryable().SingleOrDefaultAsync(x => x.Id == id, token);
        if (entity == null) throw new ArgumentNullException(entity.GetType().Name, $"Has no entity with {id} id");

        await Delete(entity, token);
        await _context.SaveChangesAsync(token);
    }

    public async Task<T?> CreateOrUpdate(T entity, CancellationToken token)
    {
        var exist = await _context.Set<T>().AsNoTracking().AsQueryable()
            .AnyAsync(x => x.Id == entity.Id, token);
        if (exist)
            await Update(entity, token);
        else
            await Create(entity, token);

        await _context.SaveChangesAsync(token);

        return await _context.Set<T>().AsNoTracking().AsQueryable()
            .SingleOrDefaultAsync(x => x.Id == entity.Id, token);
    }
}