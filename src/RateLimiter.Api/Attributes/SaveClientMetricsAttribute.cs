using Microsoft.AspNetCore.Mvc;
using RateLimiter.Filters;

namespace RateLimiter.Api.Attributes
{
    /// <summary>
    /// A saving cache attribute used create filters to decorate our controllers/actions
    /// </summary>
    public class SaveClientMetricsAttribute : TypeFilterAttribute
    {
        public SaveClientMetricsAttribute() : base(typeof(SaveClientMetricsFilter))
        {

        }
    }
}
