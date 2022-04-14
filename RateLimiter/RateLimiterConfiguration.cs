using Microsoft.Extensions.Configuration;
using RateLimiter.Models;
using System.Collections.Generic;
using System.IO;

namespace RateLimiter
{
    public static class RateLimiterConfiguration
    {
        private static bool _isInitialized;
        private static IConfigurationBuilder _builder;
        private static IConfigurationRoot _configuration;

        private static void Initialize()
        {
            if (_isInitialized) return;
            _builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);

            _configuration = _builder.Build();
            _isInitialized = true;
        }

        public static string AppName
        {
            get
            {
                Initialize();
                return _configuration.GetSection("AppName").Value;
            }
        }

        public static bool Enabled
        {
            get
            {
                Initialize();
                _ = bool.TryParse(_configuration.GetSection("Enabled").Value, out var result);
                return result;
            }
        }

        public static List<string> AllowList
        {
            get
            {
                Initialize();
                var list = new List<string>();
                _configuration.GetSection("AllowList").Bind(list);
                return list;
            }
        }

        public static List<string> BlockList
        {
            get
            {
                Initialize();
                var list = new List<string>();
                _configuration.GetSection("BlockList").Bind(list);
                return list;
            }
        }

        public static string Header
        {
            get 
            { 
                Initialize();
                return _configuration.GetSection("Header").Value;
            }
        }

        public static List<RateLimitRule> Rules
        {
            get
            {
                Initialize();
                var rules = new List<RateLimitRule>();
                _configuration.GetSection("Rules").Bind(rules);
                return rules;
            }
        }
    }
}
