using RateLimiter.Enums;
using System.Collections.Generic;

namespace RateLimiter.Interfaces
{
    public interface IExtendableRuleFactory
    {
        List<IRateLimiter> GetRulesByServiceType(ServiceType serviceType);
    }
}
