using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RateLimiter.Filters
{
    /// <summary>
    /// Authorization filter used to validate whether a access token is present
    /// </summary>
    public class AuthorizationFilter : IAuthorizationFilter
    {
        /// <summary>
        /// Called early in the filter pipeline to confirm the request is authorized
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.ContainsKey("Authorization") ||
                !context.HttpContext.Request.Headers["Authorization"][0].StartsWith("Bearer "))
                context.Result = new UnauthorizedResult();
        }
    }
}
