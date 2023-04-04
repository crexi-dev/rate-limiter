using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Contracts.Interfaces
{
    public interface IApiService
    {
        Task<string> MakeApiCallAsync(string accessToken, string resource);
    }
}
