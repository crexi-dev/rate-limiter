using NUnit.Framework;
using RateLimiter.Models;
using RateLimiter.Services;
using System.Collections.Generic;

namespace RateLimiter.Tests
{
    internal static class DummyDataHelper
    {
        internal static void FillRequests()
        {
            var cService = DependencyHelper.GetRequiredService<ICacheService>(true);

            //var client1Data = new List<RequestHistoryEModel>()
            //{ 
                
            //}
            //cService.StoreData(new )
        }
    }
}
