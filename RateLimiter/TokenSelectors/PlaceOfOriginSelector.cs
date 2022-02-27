using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.TokenSelectors
{
	public class PlaceOfOriginSelector<T> : ITokenSelector<T>
	{
		private readonly HashSet<string> _countryCodes;

		public PlaceOfOriginSelector(IEnumerable<string> countryCodes)
		{
			_countryCodes = countryCodes
				.Select(countryCode => countryCode.ToUpperInvariant())
				.ToHashSet();
		}

		public bool IsSelected(IAccessToken<T> accessToken) =>
			_countryCodes.Contains(accessToken.CountryCode.ToUpperInvariant());
	}
}
