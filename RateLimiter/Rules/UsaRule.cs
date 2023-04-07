using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    public class UsaRule : IRule
    {
        public int Limit { get; set; }

        public TimeSpan Interval { get; set; }

        public bool IsExceeded(string requestKey)
        {
            bool result = false;
            var storageItem = MemoryCache.Default.Get(requestKey);
            int count = storageItem is null ? 0 : (int)storageItem;
            if (count >= Limit)
            {
                result = true;

            }
            else
            {
                result = false;
                count++;

            }
            MemoryCache.Default.Set(requestKey, count, DateTime.Now.Add(Interval));
            return result;
        }

        //private string Key() 
        //{
        //    return "Resource" + "Client";
        //}  



    }
}
