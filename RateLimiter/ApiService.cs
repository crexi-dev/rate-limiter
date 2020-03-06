using RateLimiter.Common;
using RateLimiter.Rules;
using RateLimiter.Services;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter
{
    public class ApiService
    {
        private readonly ApiDataService _dataService;

        public ApiService(ApiDataService dataService)
        {
            _dataService = dataService;
        }
        public bool NewRequest(string token, CustomerRequest request)
        {
            var customer = GetCustomer(request.CustomerId, token);
            if (customer != null)
            {
                var rules = new List<Rule>();

                //general rule - applicable for any client in specific region 
                if(customer.Region == "UK")
                {
                    rules.Add(new MinimumIntervalRule(customer.Id, _dataService));
                }

                foreach (var rule in customer.Rules)
                {
                    switch (rule)
                    {
                        //Example for client specific rules
                        case "MaximumReqInPeriod":
                            rules.Add(new MaximumReqInPeriodRule(customer.Id, _dataService));
                            break;
                        default:
                            break;
                    }
                }

                foreach (var rule in rules)
                {
                    if (!rule.IsValidRequest())
                        return false;
                }
                _dataService.AddRequest(request);
                return true;
            }

            return false;
        }

        private Customer GetCustomer(int customerId, string token)
        {
            return _dataService.Customers.Where(c => c.Id == customerId && c.AccessToken == token).SingleOrDefault();
        }
    }
}
