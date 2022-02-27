using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.TokenMatchers
{
	public class PlaceOfOriginMatcher<T> : ITokenMatcher<T>
	{
		private readonly HashSet<string> _countryCodes;

		public PlaceOfOriginMatcher(IEnumerable<string> countryCodes)
		{
			_countryCodes = countryCodes
				.Select(countryCode => countryCode.ToUpperInvariant())
				.ToHashSet();
		}

		public bool MatchesToken(IAccessToken<T> accessToken) =>
			_countryCodes.Contains(accessToken.CountryCode.ToUpperInvariant());
	}
}
