namespace RateLimiter.TokenSelectors
{
	public static class Combination
	{
		public static ITokenSelector And(ITokenSelector a, ITokenSelector b) =>
			new TokenSelectorBuilder(accessToken => a.IsSelected(accessToken) && b.IsSelected(accessToken));

		public static ITokenSelector Or(ITokenSelector a, ITokenSelector b) =>
			new TokenSelectorBuilder(accessToken => a.IsSelected(accessToken) || b.IsSelected(accessToken));

		public static ITokenSelector Not(ITokenSelector tokenSelector) =>
			new TokenSelectorBuilder(accessToken => !tokenSelector.IsSelected(accessToken));


		#region TokenSelectorBuilder

		private delegate bool LimitSelector(IAccessToken accessToken);

		private class TokenSelectorBuilder : ITokenSelector
		{
			private readonly LimitSelector _limitSelector;

			public TokenSelectorBuilder(LimitSelector limitSelector)
			{
				_limitSelector = limitSelector;
			}

			public bool IsSelected(IAccessToken accessToken) => _limitSelector.Invoke(accessToken);
		}

		#endregion TokenSelectorBuilder
	}
}
