using RateLimiter.Models;
using System.Collections.Generic;

namespace RateLimiter.Builders
{
    public sealed class RateLimiterContextBuilder
    {
        private string resourceName = "resource1";
        private string region = "USA";
        private bool isClientAuthenticated = false;
        private bool hasSubscription = false;
        private List<string> roles = new List<string>();

        public RateLimiterContextBuilder ForResource(string name)
        {
            resourceName = name;
            return this;
        }

        public RateLimiterContextBuilder InRegion(string name)
        {
            region = name;
            return this;
        }

        public RateLimiterContextBuilder ClientAuthenticated()
        {
            isClientAuthenticated = true;
            return this;
        }

        public RateLimiterContextBuilder ClientHasSubscription()
        {
            hasSubscription = true;
            return this;
        }

        public RateLimiterContextBuilder WithRole(string role)
        {
            roles.Add(role);
            return this;
        }

        public RateLimiterContext Build() => new RateLimiterContext
        {
            ClientId = "100",
            ResourceName = resourceName,
            Region = region,
            IsClientAuthenticated = isClientAuthenticated,
            HasSubscription = hasSubscription,
            Roles = roles
        };
    }
}
