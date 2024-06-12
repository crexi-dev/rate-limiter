using System;

using Unity;

namespace RateLimiter
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class TimeSpanPassedSinceLastCallRuleAttribute : ResourceRuleAttribute
	{
		private IStorage _storage;
		private string _resource;
		private TimeSpan _interval;

		public TimeSpanPassedSinceLastCallRuleAttribute(string resource, int seconds)
		{
			_storage = Application.UnityContainer.Resolve<IStorage>();
			_resource = resource;
			_interval = TimeSpan.FromSeconds(seconds);
		}

		public override bool IsValid(IRequest payload)
		{
			IRequest request = _storage.GetLastTimeSpanRequestByResource(_resource);

			_storage.UpdateLatestRequest(_resource, payload);

			return request == null || payload.TimeStamp.Subtract(request.TimeStamp) >= _interval;
		}
	}
}
