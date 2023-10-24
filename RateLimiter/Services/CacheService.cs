using RateLimiter.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RateLimiter.Models;
using System.Threading;
using System.Linq.Expressions;

namespace RateLimiter.Services
{
    public class CacheService : ICacheService
    {
        private readonly RateDBContextBase _context;

        public CacheService(RateDBContextBase context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetData<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
            where T : class
        {
            return await _context.Set<T>().Where(expression)
                                          .ToListAsync(cancellationToken);         
        }

        public async Task<int> GetRecordsCount<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
            where T : class
        {
            return await _context.Set<T>().Where(expression)
                                          .CountAsync(cancellationToken);
        }

        public async Task StoreData<T>(T data, CancellationToken cancellationToken = default)
        {
            var res = await _context.AddAsync(data, cancellationToken);
            await _context.SaveChangesAsync();
        }

        public async Task StoreData<T>(IEnumerable<T> data, CancellationToken cancellationToken = default)
        {
            await _context.AddRangeAsync(data, cancellationToken);
            await _context.SaveChangesAsync();
        }

        public async Task<RequestHistoryEModel> GetClientLastRecord(string key, CancellationToken cancellationToken = default)
        {
            return await _context.Set<RequestHistoryEModel>()
                                 .OrderByDescending(x => x.ReqDate)
                                 .FirstOrDefaultAsync(x => x.ClientId == key, cancellationToken);
        }
    }
}
