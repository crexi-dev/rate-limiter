using System;
namespace RateLimiter.Services
{
    public  interface ILogApiHitCountService
    {
        Task<ApiHitCountLog> GetApiCounts(string key);
        Task SaveApiCounts<ApiHitCountLog>(string key, ApiHitCountLog apiHitCountLog);
    }
}

