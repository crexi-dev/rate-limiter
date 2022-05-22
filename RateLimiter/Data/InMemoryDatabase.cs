using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Data
{
    public static class InMemoryDatabase
    {
        public static Dictionary<string, Dictionary<string, List<TokenBucketModel>>> TokenBucketData = new Dictionary<string, Dictionary<string, List<TokenBucketModel>>>();
        public static Dictionary<string, Dictionary<string, List<TokenIntervalModel>>> TokenIntervalData = new Dictionary<string, Dictionary<string, List<TokenIntervalModel>>>();

    }
}


