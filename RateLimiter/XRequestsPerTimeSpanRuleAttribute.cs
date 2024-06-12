using System;

using Unity;

namespace RateLimiter
{
	public class XRequestsPerTimeSpanRuleAttribute : ResourceRuleAttribute
	{
		private IStorage _storage;
		private string _resource;
		private TimeSpan _interval;
		private int _requestLimit;

		public XRequestsPerTimeSpanRuleAttribute(string resource, int seconds, int requestLimit)
		{
			_storage = Application.UnityContainer.Resolve<IStorage>();
			_resource = resource;
			_interval = TimeSpan.FromSeconds(seconds);
			_requestLimit = requestLimit;
		}

		public override bool IsValid(IRequest payload)
		{
			_storage.AddRequestAndPurge(_resource, _interval, payload, _requestLimit);

			int count = _storage.GetXRequestsPerTimeSpanRequestCountByResource(_resource, payload, _interval);

			return count > 0 && count <= _requestLimit;
		}
	}
}
