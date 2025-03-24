using RateLimiter.Model.Resource;
using RateLimiter.Model.Resource.Config;

namespace RateLimiter.Resource
{
    public interface IServiceResourceConfigLoader
    {
        ResourceConfig GetConfig(ResourceRequest reques);
    }
}
