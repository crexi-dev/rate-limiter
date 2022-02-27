using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.RequestFilters
{
	/// <summary>
	/// Filters requests to only relevant resources.
	/// </summary>
	public class ResourceFilter<T> : IRequestFilter<T>
	{
		private readonly HashSet<string> _resources;

		public ResourceFilter(params string[] resources)
		{
			_resources = resources
				.Select(resource => resource.ToUpperInvariant())
				.ToHashSet();
		}

		public bool IsRequestIncluded(IApiRequest<T> request) =>
			_resources.Contains(request.Resource.ToUpperInvariant());
	}
}
