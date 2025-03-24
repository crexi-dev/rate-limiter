using System;

namespace RateLimiter.Log
{
    public class ServiceLogger : IServiceLogger
    {
        public void Exception(Exception ex)
        {
            if (ex != null)
            {
                Log(ex.Message);
            }
            else
            {
                Log("We encountered an unexpected exception");
            }
        }

        public void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
