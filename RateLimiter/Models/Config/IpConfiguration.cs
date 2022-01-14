using System.Collections.Generic;
using RateLimiter.Models.Config.Base;

namespace RateLimiter.Models.Config;

public class IpConfiguration : BaseConfiguration, IAllowedClientsConfig
{
    public IEnumerable<string>? AllowedIdentifiers { get; set; }
}