using Microsoft.EntityFrameworkCore;
using RateLimiter.DAL.Entities;

namespace RateLimiter.DAL.Repositories;

public class HistoryRepository : GenericRepository<History>, IHistoryRepository
{
    public HistoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<History?> GetHistoryByTokenId(Guid tokenId, CancellationToken token)
    {
        return _context.Histories.LastOrDefaultAsync(x => x.TokenId == tokenId, token);
    }
}