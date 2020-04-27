using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class API
    {
        public API() { }

        public string Get(string authToken, List<RateLimiterRule> rules = null)
        {
            var rateLimiter = RateLimiter.Instance;
            User user = AuthenticateUser(authToken);

            int requestId=123;
            if (rateLimiter.ValidateRuleList(user.Id, requestId, rules, new List<IRateLimiterFilter>() { new LocationBasedFilter() { CountryCode = user.CountryCode } }))
                return DoGetCall(user);
            else
                throw new TimeoutException("Rate limit exceeded. Please retry later."); // Can be further implemented as RateLimiter option (HardLimit or SoftLimit).
        }

        public string DoGetCall(User user)
        {
            string result = "success";

            return result;
        }

        public User AuthenticateUser(string authToken)
        {
            return new User { Id = 100, CountryCode = "US", Name = "Dana Sherman", UserIP = "127.0.0.1" };
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }

        public string UserIP { get; set; }

        public User()
        {
        }

    }
}
