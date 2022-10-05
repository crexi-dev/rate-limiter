using RateLimiter.DAL.Entities;

namespace RateLimiter.DAL.Repositories;

public interface IHistoryRepository : IGenericRepository<History>
{
    Task<History?> GetHistoryByTokenId(Guid tokenId, CancellationToken token);
}