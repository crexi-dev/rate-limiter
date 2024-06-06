using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using RateLimit.Contracts;
using RateLimit.DTO;
using System.Net.Http;

namespace RateLimit
{
	public class RateLimiterMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IEnumerable<IRule> _rules;

		public RateLimiterMiddleware(RequestDelegate next, IEnumerable<IRule> rules)
		{
			_next = next;
			_rules = rules;
		}
		public async Task Invoke(HttpContext context)
		{
			RequestDto accessToken = ParseToken(context.Request.Headers["access_token"]);

			if (!_rules.Any() || _rules.All(x => { return x.IsAccessAllowedAsync(accessToken).Result; }))
			{
				await _next(context);
			}
			context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
			await context.Response.WriteAsync("Rate limit exceeded");
			return;
		}

		//simplifying gettting Client data from token 
		private RequestDto ParseToken(StringValues stringValues)
		{
			return new RequestDto { ClientId = stringValues[0], Region = stringValues[1] };
		}
	}
}
