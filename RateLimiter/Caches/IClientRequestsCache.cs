using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Caches
{
    public interface IClientRequestsCache
    {
        Task<bool> AddClientRequestDataAsync(string key, RequestsHistoryModel data);
        Task<List<RequestsHistoryModel>> GetClientRequestsDataAsync(string key);
    }
}
