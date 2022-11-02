using System.Collections.Generic;

namespace RateLimiter.Rules
{
    public class GeneralRule
    {
        public GeneralRule()
        {
            GeneralRules = new Dictionary<string, List<Rule>>
            {
                { "/api/GetProducts",
                    new List<Rule>
                    {
                        new NumberRequestsPerTimeSpan(),
                        new PeriodBetweenRequests()
                    }
                },
                { "/api/GetCustomers",
                    new List<Rule>
                    {
                        new NumberRequestsPerTimeSpan(),
                        new PeriodBetweenRequests()
                    }
                },
                { "/api/GetEmployees",
                    new List<Rule>
                    {
                        new NumberRequestsPerTimeSpan(),
                        new PeriodBetweenRequests()
                    }
                }
            };
        }

        public Dictionary<string, List<Rule>> GeneralRules { get; set; }
    }
}
