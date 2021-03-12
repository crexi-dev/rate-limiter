using RateLimiter.Interfaces;
using System;

namespace RateLimiter
{
    public class NullableResource : IEvaluate
    {
        private static NullableResource _instance;
        private NullableResource()
        { }

        public static NullableResource Instance
        {
            get
            {
                if (_instance == null)
                {
                    return new NullableResource();
                }
                   
                return _instance;
            }
        }

        public bool CanGoThrough(DateTimeOffset requestDateTimeOffset) => false;
    }
}
