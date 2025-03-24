using RateLimiter.Model.Data;
using RateLimiter.Model.Resource;
using RateLimiter.Model.Resource.Config;
using RateLimiter.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Data
{
    public class RepositoryRateLimiter : IRepositoryRateLimiter
    {
        //Likely a combination of cache (something like redis) and persisted data but we will just store some data in memory for this poc
        //Mock persisted data store
        private List<ResourceConfig> ResourceConfigurations = new List<ResourceConfig>();
        private List<ResourceRequestLog> RequestLogs = new List<ResourceRequestLog>();        
        //End mock persisted data
        
        public ResourceConfig GetResourceConfig(EnumResource resource)
        {
            return ResourceConfigurations.First(c => c.Resource == resource);
        }

        public void CreateResourceConfig(ResourceConfig resourceConfig) 
        {
            ResourceConfigurations.Add(resourceConfig);
        }        

        public void LogResourceRequestAttempt(ResourceRequest request, ResponseRateLimitValidation response)
        {
            RequestLogs.Add(new ResourceRequestLog
            {
                CreateDate = DateTime.Now,
                Token = request.Token,
                IpAddress = request.IpAddress,                
                ResourceConfigId = response.ResourceConfigId,
                ResourceId = (int)request.Resource,
                ResponseCode = response.ResponseCode,
                ResponseMessage = response.ResponseMessage
            });
        }

        public int GetResourceRequestCount(EnumResource resource, string token, TimeSpan lookbackTimespan)
        {
            var lookbackDate = DateTime.Now.Subtract(lookbackTimespan);

            return RequestLogs.Where
            (
                r => r.ResourceId == (int)resource
                && r.Token == token
                && r.CreateDate >= lookbackDate
            ).Count();
        }

        public DateTime GetLastResourceRequest(EnumResource resource, string token)
        {
            return RequestLogs
                .OrderByDescending(r => r.CreateDate)
                .Select(r => r.CreateDate)
                .FirstOrDefault();
        }

        public List<ResourceRequestLog> GetRequestLogAll()
        {
            return RequestLogs;
        }
    }
}
