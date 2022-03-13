using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RateLimiter.Filters
{
    public class AuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var cookies = context.HttpContext.Request.Cookies;

            if (cookies.ContainsKey("AuthToken") == false)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
