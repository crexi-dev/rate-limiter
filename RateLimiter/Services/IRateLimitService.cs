using RateLimiter.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Services
{
    public interface IRateLimitService
    {
        Task<bool> ValidateRequestAsync(string key, string region, IList<RequestAttributeDataModel> data);
        Task<bool> AddClientRequestAsync(string key, string region);
    }
}
