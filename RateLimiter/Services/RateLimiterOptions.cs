using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Services
{
    public class RateLimiterOptions
    {
        public Dictionary<string, RateLimiterPolicy> Policies { get; set; } = new Dictionary<string, RateLimiterPolicy>();

        public void AddPolicy(string name, RateLimiterPolicy policy) 
        {
            Policies.Add(name, policy);
        }

        public void AddPolicy(string name, Action<RateLimiterPolicyBuilder> builder)
        {
            var policyBuilder = new RateLimiterPolicyBuilder();
            builder.Invoke(policyBuilder);

            var builtPolicy = policyBuilder.Build();
            builtPolicy.Name = name;
            AddPolicy(name, builtPolicy);
        }

        public RateLimiterOptions()
        {

        }
    }
}
