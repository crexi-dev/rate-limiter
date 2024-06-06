using RateLimit.Contracts;
using RateLimit.DTO;
using RateLimit.Restictions;

namespace RateLimit.Rules
{
	public class RuleB : IRule
	{
		private readonly TimeSpanPassedSinceLastCallRestictions _carerntRestrictions;

		public RuleB(IEnumerable<IRestriction> behaviour)
		{
			_carerntRestrictions = behaviour.OfType<TimeSpanPassedSinceLastCallRestictions>().First();
		}

		public async Task<bool> IsAccessAllowedAsync(RequestDto request)
		{
			return await _carerntRestrictions.IsPassedAsync(request);
		}
	}
}
