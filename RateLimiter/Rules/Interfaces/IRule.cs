using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RateLimiter.Attributes.Interfaces;

namespace RateLimiter.Rules.Interfaces;

public interface IRule
{
    public Task<bool> IsRestrict(HttpContext context);
    public void SetParameters(IRateLimiterAttribute data);
}

public interface IRule<T> : IRule where T : IRateLimiterAttribute
{
    public T? Parameters { get; set; }
}