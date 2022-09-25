using Microsoft.Extensions.Options;
using RateLimiter.Models;
using System.Threading.Tasks;

namespace RateLimiter.Middleware
{
    public class RulesProvider : IRulesProvider
	{
		private readonly RateLimitRuleOptions _options;
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
