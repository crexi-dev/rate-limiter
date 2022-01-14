using System.Collections.Generic;

namespace RateLimiter.Models.Config.Base
{
    public interface IAllowedClientsConfig
    {
        public IEnumerable<string>? AllowedIdentifiers { get; set; }
    }
}