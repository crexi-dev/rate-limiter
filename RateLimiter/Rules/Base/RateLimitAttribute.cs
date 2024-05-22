namespace RateLimiter.Rules.Base;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public abstract class RateLimitAttribute : Attribute
{
    public string CountryCode { get; set; }

    public abstract Task<bool> ValidateAsync(IDistributedCache cache, HttpContext context);
}