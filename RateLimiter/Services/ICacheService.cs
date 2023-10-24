using RateLimiter.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Services
{
    public interface ICacheService
    {
        Task<IEnumerable<T>> GetData<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
             where T : class;

        Task<int> GetRecordsCount<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
            where T : class;

        Task StoreData<T>(T data, CancellationToken cancellationToken = default);

        Task StoreData<T>(IEnumerable<T> data, CancellationToken cancellationToken = default);

        //TO DO - Generic
        Task<RequestHistoryEModel> GetClientLastRecord(string key, CancellationToken cancellationToken = default);
    }
}
