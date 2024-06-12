using System;
using System.Collections.Generic;
using System.Linq;

using Unity;

namespace RateLimiter
{
	public class Storage : IStorage
	{
		private IConfiguration _configuration;
		private Dictionary<string, IRequest> _requestStorage;
		private Dictionary<string, List<IRequest>> _requestCountStorage;

		public Storage()
		{
			_configuration = Application.UnityContainer.Resolve<IConfiguration>();
			_requestStorage = new Dictionary<string, IRequest>();
			_requestCountStorage = new Dictionary<string, List<IRequest>>();
		}

		public void UpdateLatestRequest(string resource, IRequest request)
		{
			IRequest lastRequest = null;

			if (_requestStorage.TryGetValue(resource, out lastRequest))
			{
				_requestStorage[resource] = request;
			}
			else
			{
				_requestStorage.Add(resource, request);
			}
		}

		public void AddRequestAndPurge(string resource, TimeSpan interval, IRequest request, int requestLimit)
		{
			List<IRequest> requests = null;

			if (_requestCountStorage.TryGetValue(resource, out requests))
			{
				if (requests.Count() > requestLimit)
				{
					var obsolete = requests
						.Where(r => r.TimeStamp < r.TimeStamp.Subtract(interval))
						.ToList();

					if (obsolete.Any())
					{
						foreach (var t in obsolete)
						{
							requests.Remove(t);
						}
					}
				}

				requests.Add(request);

				_requestCountStorage[resource] = requests;
			}
			else
			{
				requests = new List<IRequest>();

				requests.Add(request);

				_requestCountStorage.Add(resource, requests);
			}
		}

		public IRequest GetLastTimeSpanRequestByResource(string resource)
		{
			IRequest lastRequest = null;

			if (_requestStorage.TryGetValue(resource, out lastRequest))
			{
				return lastRequest;
			}

			return null;
		}

		public int GetXRequestsPerTimeSpanRequestCountByResource(string resource, IRequest request, TimeSpan interval)
		{
			List<IRequest> requests = null;

			if (_requestCountStorage.TryGetValue(resource, out requests))
			{
				int count = requests.Count(i => i.TimeStamp >= request.TimeStamp.Subtract(interval));

				return count;
			}

			return 0;
		}
	}
}
