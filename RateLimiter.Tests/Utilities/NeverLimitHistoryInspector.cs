using RateLimiter.HistoryInspectors;
using System.Collections.Generic;

namespace RateLimiter.Tests.Utilities
{
	/// <summary>
	/// A default history inspector that never rate limits the user.
	/// </summary>
	internal class NeverLimitHistoryInspector<T> : IHistoryInspector<T>
	{
		public bool IsRateLimited(IEnumerable<IApiRequest<T>> history) => false;
	}
}
