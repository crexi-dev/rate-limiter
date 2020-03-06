using RateLimiter.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Services
{
	public class ApiDataService
	{
		public List<Customer> Customers { get; }

		public List<CustomerRequestLog> RequestLogs { get; }

		public ApiDataService()
		{
			//Test Customer Data;
			Customers = new List<Customer>();
			Customers.Add(new Customer { Id = 1, AccessToken = "Client A", Region = "USA", Rules = new[] { "MaximumReqInPeriod" } });
			Customers.Add(new Customer { Id = 2, AccessToken = "Client B", Region = "UK", Rules = new[] { "MaximumReqInPeriod" } });

			//Test Customer Request Data;
			RequestLogs = new List<CustomerRequestLog>();
			for (int i = 1; i <= 4; i++)
			{
				RequestLogs.Add(new CustomerRequestLog
				{
					Id = i,
					CustomerId = (i + 1) % 2 + 1,
					Request = "Req " + i,
					RequestTime = DateTime.Now.AddSeconds(-1 * (4 - i))
				}) ;
			}
		}

		public IList<CustomerRequestLog> GetRequestLogs(int customerId)
		{
			return RequestLogs.Where(l => l.CustomerId == customerId).ToList();
		}

		public void AddRequest(CustomerRequest req)
		{
			var maxId = RequestLogs.Max(l => l.Id);
			var log = new CustomerRequestLog { Id = maxId + 1, CustomerId = req.CustomerId, Request = req.Request, RequestTime = DateTime.Now };
			RequestLogs.Add(log);
		}
	}
}
