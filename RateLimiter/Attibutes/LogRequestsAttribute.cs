using Microsoft.AspNetCore.Mvc;
using RateLimiter.Filters;

namespace RateLimiter.Attibutes
{
    public class LogRequestsAttribute : TypeFilterAttribute
    {
        public LogRequestsAttribute() : base(typeof(LogRequestsFilter))
        {

        }
    }
}
