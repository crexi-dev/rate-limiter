
namespace RateLimiter
{
    public class ClientConfig
    {
        public MaxRequestSettings MaxRequestPerTime { get; set; }
        public PassedSinceLastCallSettings PassedSinceLastCall { get; set; }
    }

    public class ConfigModel
    {
        public ConfigSettings<MaxRequestSettings> MaxRequestPerTime { get; set; }
        public ConfigSettings<PassedSinceLastCallSettings> PassedSinceLastCall { get; set; }
    }

    public class ConfigSettings<T>
    {
        public T DefaultSetting { get; set; }
        public T[] RegionSetting { get; set; }
    }
    public class MaxRequestSettings : RegionSetting
    {
        public int Milliseconds { get; set; }
        public int Max { get; set; }
    }

    public class PassedSinceLastCallSettings : RegionSetting
    {
        public int Milliseconds { get; set; }
    }

    public class RegionSetting
    {
        public string Region { get; set; }
    }
}
