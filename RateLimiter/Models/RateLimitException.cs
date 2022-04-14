using System;

namespace RateLimiter.Models
{
    public class RateLimitException : ExceptionBase
    {
        public RateLimitException(string customMessage) : base(customMessage) { }

        public RateLimitException(string customMessage, Exception exception) : base(customMessage, exception) { }
    }

    public class ExceptionBase : Exception
    {
        /// <summary>
        /// Automatically Log an error to Logging Platform
        /// </summary>
        /// <param name="customMessage"></param>
        public ExceptionBase(string customMessage) : base(customMessage)
        {
            //if (Log.IsEnabled(LogEventLevel.Error))
            //{
            //    Log.Error(customMessage);
            //}
        }

        /// <summary>
        /// Automatically Log an error to Logging Platform
        /// </summary>
        /// <param name="customMessage"></param>
        /// <param name="exception"></param>
        public ExceptionBase(string customMessage, Exception exception) : base(customMessage, exception)
        {
            //if (Log.IsEnabled(LogEventLevel.Error))
            //{
            //    Log.Error($"{customMessage}: {exception.DetailedException().Message}");
            //}
        }
    }
}
