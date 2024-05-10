using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Processor
{
    public interface IRateLimitProcessor
    {
        bool Process(HttpContext httpContext);
    }
}
