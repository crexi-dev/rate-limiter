using RateLimit.Contracts;
using RateLimit.DTO;
using RateLimit.Restictions;

namespace RateLimit.Rules
{
	public class RuleC : IRule
	{
		private readonly TimeSpanPassedSinceLastCallRestictions _lastCallRestiction;
		private readonly XRequestPerDatespanRestictions _xRequestPerTimeRestiction;

		public RuleC(IEnumerable<IRestriction> restrictions)
		{
			_lastCallRestiction = restrictions.OfType<TimeSpanPassedSinceLastCallRestictions>().First();
			_xRequestPerTimeRestiction = restrictions.OfType<XRequestPerDatespanRestictions>().First();
		}

		public async Task<bool> IsAccessAllowedAsync(RequestDto request)
		{
			if (request.Region.Equals(RegionEnum.US.ToString(), StringComparison.OrdinalIgnoreCase))
				return await _xRequestPerTimeRestiction.IsPassedAsync(request);

			else if (request.Region.Equals(RegionEnum.EU.ToString(), StringComparison.OrdinalIgnoreCase))
				return await _lastCallRestiction.IsPassedAsync(request);

			return false;
		}
	}
}
