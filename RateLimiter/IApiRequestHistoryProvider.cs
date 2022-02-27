using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter
{
	/// <summary>
	/// A mechanism for accessing the history of a user given their
	/// access token. This is needed to track incoming requests in order
	/// to enforce rate limits.
	/// 
	/// The rate limiter is ignorant of the data source for this history. It
	/// could be in local memory, running in a different process, or on a different
	/// machine. For this reason, the fetch is done asynchronously in case of
	/// a slow response.
	/// 
	/// Note that pruning the history is an important function for performance
	/// but is outside the scope of this library and should be implemented elsewhere.
	/// </summary>
	public interface IApiRequestHistoryProvider<T>
	{
		/// <summary>
		/// Get the request history for a user given their access token.
		/// </summary>
		Task<IEnumerable<IApiRequest<T>>> GetApiRequestHistory(
			IAccessToken<T> accessToken,
			CancellationToken cancellationToken = default);
	}
}
