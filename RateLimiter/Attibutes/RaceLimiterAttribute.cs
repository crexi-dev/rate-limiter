using Microsoft.AspNetCore.Mvc;
using RateLimiter.Data.Enums;
using RateLimiter.Filters;

namespace RateLimiter.Attibutes
{
    public class RaceLimiterAttribute : TypeFilterAttribute
    {
        public RaceLimiterAttribute(RateLimiterTypeEnum type) : base(typeof(RateLimiterFilter))
        {
            Arguments = new object[] { type };
        }
    }
}
