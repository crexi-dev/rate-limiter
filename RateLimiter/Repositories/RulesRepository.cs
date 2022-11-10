using RateLimiter.Constants;
using RateLimiter.Enums;
using RateLimiter.Rules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Repositories
{
    public class RulesRepository : IRulesRepository
    {
        public Rule GetRule(string endpoint, Location location)
        {
            if (string.IsNullOrEmpty(endpoint))
            {
                throw new Exception("Unable to get rules, rule endpoint is not found!");
            }

            var rules = RulesConfiguration().GetValueOrDefault(endpoint);

            return rules.FirstOrDefault(x => x.Location == location);
        }

        private Dictionary<string, List<Rule>> RulesConfiguration()
        {
            var rules = new Dictionary<string, List<Rule>>
            {
                { 
                    EndpointConstatns.GetProducts,
                    new List<Rule>
                    {
                        new NumberRequestsPerTimeSpan()
                        {
                            Period = TimeSpan.FromSeconds(20),
                            Limit = 2,
                            Location = Location.US
                        },
                        new PeriodBetweenRequests()
                        {
                            Period = TimeSpan.FromSeconds(30),
                            Limit = 5,
                            Location = Location.EU
                        }
                    }
                },
                { 
                    EndpointConstatns.GetCustomers,
                    new List<Rule>
                    {
                        new NumberRequestsPerTimeSpan()
                        {
                            Period = TimeSpan.FromMinutes(10),
                            Limit = 3,
                            Location = Location.US
                        },
                        new PeriodBetweenRequests()
                        {
                            Period = TimeSpan.FromMinutes(15),
                            Limit = 5,
                            Location = Location.EU
                        }
                    }
                },
                { 
                    EndpointConstatns.GetEmployees,
                    new List<Rule>
                    {
                        new NumberRequestsPerTimeSpan()
                        {
                            Period = TimeSpan.FromMinutes(30),
                            Limit = 5,
                            Location = Location.US
                        },
                        new PeriodBetweenRequests()
                        {
                            Period = TimeSpan.FromHours(1),
                            Limit = 10,
                            Location = Location.EU
                        }
                    }
                }
            };

            return rules;
        }
    }
}
