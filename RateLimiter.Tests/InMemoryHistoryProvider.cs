using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
	internal class InMemoryHistoryProvider<T> : IApiRequestHistoryProvider<T> where T : notnull
	{
		private readonly Dictionary<T, List<IApiRequest<T>>> _history;

		public InMemoryHistoryProvider()
		{
			_history = new Dictionary<T, List<IApiRequest<T>>>();
		}

		public async Task<IEnumerable<IApiRequest<T>>> GetApiRequestHistory(IAccessToken<T> accessToken, CancellationToken cancellationToken = default) =>
			await Task.FromResult(_history.GetValueOrDefault(accessToken.UserId, new List<IApiRequest<T>>()).AsReadOnly());

		public void Record(IApiRequest<T> request)
		{
			_history.TryAdd(request.UserId, new List<IApiRequest<T>>());
			_history[request.UserId].Add(request);
		}
	}
}
