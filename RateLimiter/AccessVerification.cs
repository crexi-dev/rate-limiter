using RateLimiter.ConfigurationHelper;
using RateLimiter.Interface;
using RateLimiter.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter
{
    public class AccessVerification
    {
        private readonly IRateLimiterRepository _rateLimiterRepository;       
        private IConfigurationHelper _configurationHelper;

        public AccessVerification(IRateLimiterRepository rateLimiterRepositor, IConfigurationHelper configurationHelper)
        { 
            _rateLimiterRepository = rateLimiterRepositor;
            _configurationHelper = configurationHelper;
        }

        public bool EnableAccess(IEnumerable<IRateLimiterVerification> verificationRules , string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }    

            if (verificationRules == null)
            {
                return true;
            }

            var beginningTime = DateTime.Now.Subtract(new TimeSpan(0, 0, 0, int.Parse(_configurationHelper.MaxRequestTimespan)));
            var requets = _rateLimiterRepository.RetrieveRequestByBeginningTime<Request>(token, beginningTime);

            if (requets == null || requets.Count() == 0)
            {
                return true;
            }           

            foreach (var rule in verificationRules)
            {
                if (rule.VerifyAccess(requets) == false)
                { 
                    return false;
                }
            }            
            return true;
        }
    }
}
