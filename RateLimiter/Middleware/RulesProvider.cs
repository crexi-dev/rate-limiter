using Microsoft.Extensions.Options;
using RateLimiter.Models;
using System.Threading.Tasks;

namespace RateLimiter.Middleware
{
    public class RulesProvider : IRulesProvider
	{
		private readonly RateLimitRuleOptions _options;
		// this could get the configured rules from an external source (an api?), then cache them?
		// that'll allow modifications to the rules without requiring any changes on the Api
		public RulesProvider(IOptions<RateLimitRuleOptions> options)
		{
			_options = options.Value;
		}
		public async Task<RateLimitRuleOptions> GetConfiguredRulesAsync()
		{
			await Task.CompletedTask;
			return _options;
		}
	}
}
