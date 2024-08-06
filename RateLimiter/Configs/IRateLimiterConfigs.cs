using RateLimiter.Models;

namespace RateLimiter.Configs
{
    public interface IRateLimiterConfigs
    {
        public ConfigValues? BindConfig();
    }
}