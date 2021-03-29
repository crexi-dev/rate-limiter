using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Domain.Aggregate
{
    public interface IResourceRepository : IDisposable
    {
        Resource Get(string key);
    }
}
