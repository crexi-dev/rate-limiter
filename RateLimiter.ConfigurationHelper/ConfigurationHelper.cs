using Microsoft.Extensions.Configuration;

namespace RateLimiter.ConfigurationHelper
{
    public interface IConfigurationHelper
    {
        string MaxRequestCountPerTimespan { get; }
        string MaxRequestTimespan { get; }
        string LastRequestTimePeriod { get; }
    }

    public class ConfigurationHelper : IConfigurationHelper
    {
        public string MaxRequestCountPerTimespan => _appSettings["MaxRequestCountPerTimespan"];
        public string MaxRequestTimespan => _appSettings["MaxRequestTimespan"];
        public string LastRequestTimePeriod => _appSettings["LastRequestTimePeriod"];

        private readonly IConfigurationSection _appSettings;
        public ConfigurationHelper(IConfiguration configuration)
        {
            _appSettings = configuration.GetSection("appSettings");
        }
    }
}
