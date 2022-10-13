using System;
using System.Threading.Tasks;

namespace RateLimiter
{
    // Should be registered as singleton
    internal interface IRequestStatisticsDataSource
    {
        Task AddRequestAsync(Request request);

        Task<int> GetNumberOfRequestsPassedAsync(Guid clientId, DateTime startTime, DateTime endTime);

        Task<Request> GetMostRecentRequestAsync(Guid clientId);
    }
}
