using RateLimiter.Models;
using System.Collections.Generic;

namespace RateLimiter
{
    public class RateLimitingConfigurations
    {
        public List<RateLimitConfiguration> Configurations { get; set; }
        public List<RateLimitCombination> Combinations { get; set; }

        public RateLimitingConfigurations(List<RateLimitConfiguration> configurations)
        {
            Configurations = configurations;
        }

        public RateLimitingConfigurations(List<RateLimitConfiguration> configurations, List<RateLimitCombination> combinations)
        {
            Configurations = configurations;
            Combinations = combinations;
        }
    }

    public class RateLimitConfiguration
    {
        public string Title { get; set; }
        public List<RateLimiting> Limits { get; set; }
        public RateLimitConfiguration(string title, List<RateLimiting> limits)
        {
            Title = title;
            Limits = limits;
        }
    }

    public class RateLimitCombination
    {
        public List<string> Titles { get; set; }
        public RateLimitingCombinationType CombinationType { get; set; }
        public RateLimitCombination(List<string> titles, RateLimitingCombinationType combinationType)
        {
            Titles = titles;
            CombinationType = combinationType;
        }
    }
}
