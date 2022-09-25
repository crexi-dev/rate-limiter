using RateLimiter.Interfaces;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Services
{
    public class RateLimiterHandlerContext
    {
        public User User { get; set; }

        public string RequestPath { get; set; }
        public RateLimiterPolicy Policy { get; set; }

        public IDateTimeWrapper RequestDate { get; set; }

        private readonly HashSet<IRateLimiterRequirement> _pendingRequirements;
        private bool _failCalled;
        private bool _succeedCalled;
        public object? Resource;

        public virtual IEnumerable<IRateLimiterRequirement> Requirements { get; }

        public void Succeed(IRateLimiterRequirement requirement)
        {
            _succeedCalled = true;
            _pendingRequirements.Remove(requirement);
        }

        public virtual void Fail()
        {
            _failCalled = true;
        }

        public bool RequirementsMet { 
            get 
            { 
                return !_failCalled && _pendingRequirements.Count == 0; 
            } 
        }

        public RateLimiterHandlerContext(RateLimiterPolicy policy, IEnumerable<IRateLimiterRequirement> requirements, User user, IDateTimeWrapper requestDate, string requestPath)
        {
            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            if (requirements == null)
            {
                throw new ArgumentNullException(nameof(requirements));
            }

            Policy = policy;
            Requirements = requirements;
            User = user;
            RequestPath = requestPath;
            RequestDate = requestDate;
            _pendingRequirements = new HashSet<IRateLimiterRequirement>(requirements);
        }
    }
}
