using RateLimiter.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.BL
{
    public interface IRequestRepository
    {
        Task<ICollection<Request>> Get(string accessToken);
        Task Add(Request requestHistory);
    }
}
