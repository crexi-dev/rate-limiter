using System.Linq;
using RateLimiter;

namespace DemoApi
{
    public static class Config
    {
        public static ConfigModel Model { get; set; }

        public static ClientConfig GetClientConfig(ThrottleSettings settings) 
        {
            var result = new ClientConfig();
            if (settings.ThrottleMaxRequestPerTime.allow) 
            {
                result.MaxRequestPerTime = GetSettignsOrDefault(Model.MaxRequestPerTime, settings.ThrottleMaxRequestPerTime.region);
            }

            if (settings.ThrottlePassedSinceLastRequest.allow) 
            {
                result.PassedSinceLastCall = GetSettignsOrDefault(Model.PassedSinceLastCall, settings.ThrottlePassedSinceLastRequest.region);
            }

            return result;
        }

        private static T GetSettignsOrDefault<T>(ConfigSettings<T> settings, string region) where T : RegionSetting
        {
            if (region != null)
            {
                var forRegionSettings = settings.RegionSetting.FirstOrDefault(el => el.Region == region);
                return forRegionSettings != null ? forRegionSettings : settings.DefaultSetting;
            }
            else
            {
                return settings.DefaultSetting;
            }
        }
    }
}
