using System;

namespace RateLimiter.Log
{
    public interface IServiceLogger
    {
        public void Log(string message);
        public void Exception(Exception ex);
    }
}
