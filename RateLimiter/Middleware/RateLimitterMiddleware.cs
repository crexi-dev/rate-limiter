using Microsoft.AspNetCore.Http;
using RateLimiter.Builders;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RateLimiter.Middleware
{
    public class RateLimitterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRulesProvider _rulesProvider;
        private readonly IClientRequestResolver _clientRequestResolver;
        private readonly IRuleRunnersBuilder _ruleRunnersBuilder;
        public RateLimitterMiddleware(RequestDelegate next, IRulesProvider rulesProvider, IClientRequestResolver clientRequestResolver, IRuleRunnersBuilder ruleRunnersBuilder)
        {
            _next = next;
            _rulesProvider = rulesProvider;
            _clientRequestResolver = clientRequestResolver;
            _ruleRunnersBuilder = ruleRunnersBuilder;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var rateLimitRuleOptions = await _rulesProvider.GetConfiguredRulesAsync();

            // return if no rules are configured
            if (!rateLimitRuleOptions?.Rules.Any() ?? true)
            {
                await _next(context);
                return;
            }

            // get client request data from HttpContext
            var clientRequest = await _clientRequestResolver.ResolveClientRequestAsync(context);

            // get all the rule runners
            var ruleRunners = _ruleRunnersBuilder.GetRuleRunners(rateLimitRuleOptions, clientRequest);

            if(!ruleRunners?.Any() ?? true)
            {
                await _next(context);
                return;
            }

            foreach(var ruleRunner in ruleRunners)
            {
                var result = await ruleRunner.RunAsync(clientRequest);

                if (!result.IsSuccess)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync(result.ErrorMessage);
                    return;
                }
            }

            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
    }

    
    
}
