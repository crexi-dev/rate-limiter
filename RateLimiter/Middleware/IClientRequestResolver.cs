using Microsoft.AspNetCore.Http;
using RateLimiter.Models;
using System.Threading.Tasks;

namespace RateLimiter.Middleware
{
    public interface IClientRequestResolver
    {
        Task<ClientRequest> ResolveClientRequestAsync(HttpContext context);
    }
}
