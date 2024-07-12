using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using RateLimiter.Rules;

namespace RateLimiter;

public class RateLimitingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var controllerActionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
        if (controllerActionDescriptor != null)
        {
            var controllerRules = controllerActionDescriptor.ControllerTypeInfo
                .GetCustomAttributes(typeof(RuleAttribute), true)
                .OfType<RuleAttribute>();
            var actionRules = controllerActionDescriptor.MethodInfo
                .GetCustomAttributes(typeof(RuleAttribute), true)
                .OfType<RuleAttribute>();

            foreach (var rule in controllerRules.Concat(actionRules))
            {
                var isAllowed = await rule.IsAllowedAsync(context.RequestServices, context.RequestAborted);
                if (!isAllowed)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsync("Rate limit exceeded");
                    return;
                }
            }
        }

        await next(context);
    }
}