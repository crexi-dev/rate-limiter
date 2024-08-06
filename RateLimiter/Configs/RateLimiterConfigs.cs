using Microsoft.Extensions.Configuration;
using RateLimiter.Models;
using System;
using System.IO.Pipes;

namespace RateLimiter.Configs
{
    public class RateLimiterConfigs : IRateLimiterConfigs
    {
        /// <summary>
        /// bind config values, 
        /// </summary>
        /// <returns>Enabled? - MaxAllowed - PerSeconds window Amount</returns>
        public ConfigValues? BindConfig()
        {
            try
            {
                IConfigurationRoot config = new ConfigurationBuilder()
                    .AddJsonFile("rate-limiter-rules-dev", optional: false)
                    .Build();
                
                // get config values
                bool? isEnabled = bool.Parse(config?.GetSection("RateLimiterRules:Enabled")?.Value?.ToLower());
                int? MaxCalls = int.Parse(config?.GetSection("RateLimiterRules:CallsLimiter:MaxAllowedCalls")?.Value);
                int? PerSeconds = int.Parse(config?.GetSection("RateLimiterRules:TimeLimiter:PerTimeFrame")?.Value);

                /// return new values
                return new ConfigValues()
                {
                    Enabled = isEnabled,
                    MaxAllowed = MaxCalls,
                    TimeFrame = PerSeconds
                };
            }
            catch (Exception ex)
            {
                // error// missing configs /// log, and continue
            }

            return null;
        }
    }
}
