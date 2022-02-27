using System.Collections.Generic;

namespace RateLimiter.HistoryInspectors
{
	/// <summary>
	/// Inspects a user's request history to determine if they should be rate limited.
	/// </summary>
	public interface IHistoryInspector<T>
	{
		/// <summary>
		/// Determines whether a user should be rate limited given their request history.
		/// </summary>
		/// <param name="history">The user's past incoming API requests.</param>
		/// <returns>True if the user should be rate limited, false otherwise.</returns>
		bool IsRateLimited(IEnumerable<IApiRequest<T>> history);
	}
}
