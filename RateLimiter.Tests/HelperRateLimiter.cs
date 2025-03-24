using RateLimiter.Data;
using RateLimiter.Log;
using RateLimiter.Model.Resource.Config;
using RateLimiter.Model.Rule.Config;
using RateLimiter.Resource;
using RateLimiter.Tracking;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

namespace RateLimiter.Tests
{
    internal class HelperRateLimiter
    {
        public const string EU_TOKEN = "eu-asdf-123-asdf";
        public const string TOKEN = "xxvb-66-yui";

        public const string IP_WHITE_LIST = "68.7.53.27";
        public const string IP_BLACK_LIST = "68.7.53.99";

        public const int DEFAULT_TIMESPAN_SECONDS = 30;
        public const int DEFAULT_REQUEST_THRESHOLD = 5;

        //Assuming you wire this up in a startup class you would hook the DI but just hand rolling instances for the sake of demonstration
        public static ServiceRateLimiter GetServiceRateLimiter()
        {
            var repo = new RepositoryRateLimiter();
            PopulateResourceConfigurations(repo);

            return new ServiceRateLimiter(
                new ServiceResourceConfigLoader(repo),
                repo,
                new ServiceLogger(),
                new ServiceRequestTracker(repo));
        }

        private static void PopulateResourceConfigurations(RepositoryRateLimiter repo)
        {
            var configDefault = new ResourceConfig
            {
                ConfigId = 1,                
                Resource = Model.Resource.EnumResource.GetListings,
                Rules = new List<RuleConfigBase>
                {
                    new RuleConfigIpBlackList
                    { 
                        IpAddresses = new List<string> { IP_BLACK_LIST }
                    },
                    new RuleConfigTimespanThreshold
                    {
                        LastCallThreshold = new System.TimeSpan(0,0, DEFAULT_TIMESPAN_SECONDS),
                        MaxThreshold = DEFAULT_REQUEST_THRESHOLD
                    }
                }
            };

            var configNewResource = new ResourceConfig
            {
                ConfigId = 2,                
                Resource = Model.Resource.EnumResource.GetListingDetail,
                Rules = new List<RuleConfigBase>
                {                    
                    new RuleConfigIpWhiteList
                    { 
                        IpAddresses = new List<string> { IP_WHITE_LIST }
                    }
                }
            };

            var configResourceMaintenance = new ResourceConfig
            {
                ConfigId = 3,                
                Resource = Model.Resource.EnumResource.CreateListingInquiry,
                Rules = new List<RuleConfigBase>
                {
                    new RuleConfigEnabledResource
                    {
                        Enabled = false
                    }
                }
            };

            repo.CreateResourceConfig(configDefault);
            repo.CreateResourceConfig(configNewResource);
            repo.CreateResourceConfig(configResourceMaintenance);
        }

        public static void OutputResponse(object response)
        {
            if (response == null)
                response = "response is emtpy";

            var lineBreak = System.Environment.NewLine;
            var seperator = $"{lineBreak}##############################################################################{lineBreak}";
            
            Debug.WriteLine(seperator);
            Debug.WriteLine(JsonSerializer.Serialize(response));
            Debug.WriteLine(seperator);
        }
    }
}
