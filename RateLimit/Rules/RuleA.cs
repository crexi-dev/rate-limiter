using RateLimit.Contracts;
using RateLimit.DTO;
using RateLimit.Restictions;

namespace RateLimit.Rules
{
	public class RuleA : IRule
	{
		private readonly XRequestPerDatespanRestictions _carerntRestrictions;

		public RuleA(IEnumerable<IRestriction> restrictions)
		{
			_carerntRestrictions = restrictions.OfType<XRequestPerDatespanRestictions>().First();
		}


		public async Task<bool> IsAccessAllowedAsync(RequestDto request)
		{
			return await _carerntRestrictions.IsPassedAsync(request);
		}
	}
}
