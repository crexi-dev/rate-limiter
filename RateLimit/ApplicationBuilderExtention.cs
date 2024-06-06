using Microsoft.AspNetCore.Builder;

namespace RateLimit
{
	public static class ApplicationBuilderExtention
	{
		public static void UseRateLimiterMiddleware(this IApplicationBuilder app)
		{
			app.UseMiddleware<RateLimiterMiddleware>();
		}
	}
}