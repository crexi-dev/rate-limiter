using RateLimiter.HistoryInspectors;
using System;
using System.Collections.Generic;

namespace RateLimiter.Tests.Utilities
{
	internal class ThrowExceptionInspector<T> : IHistoryInspector<T>
	{
		public bool IsRateLimited(IEnumerable<IApiRequest<T>> history)
		{
			throw new NotImplementedException();
		}
	}
}
