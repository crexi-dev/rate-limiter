using Microsoft.Extensions.Options;
using RateLimit.Contracts;
using RateLimit.DTO;
using RateLimit.Options;

namespace RateLimit.Restictions
{
	public class TimeSpanPassedSinceLastCallRestictions : IRestriction
	{
		private readonly IOptions<RuleOptions> _options;
		private readonly ILimitService _limitService;

		public TimeSpanPassedSinceLastCallRestictions(IOptions<RuleOptions> options, ILimitService limitService)
		{
			_options = options;
			_limitService = limitService;
		}
		public async Task<bool> IsPassedAsync(RequestDto request)
		{
			return await _limitService.IsAccessAllowedAsync(1, _options.Value.LastCallPeriod, request.ClientId);
		}
	}
}
