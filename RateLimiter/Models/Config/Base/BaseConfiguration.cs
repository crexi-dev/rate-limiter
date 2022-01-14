using System.Collections.Generic;

namespace RateLimiter.Models.Config.Base;

public class BaseConfiguration
{
    public List<Policy>? Policies { get; set; }
}