using Microsoft.Extensions.Logging;
using System;

namespace RateLimiter.Tests
{
    /// <summary>
    /// For testing, this minimal implementation of ILogger<> writes all logs to the console 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class DefaultLogService<T> : ILogger<T>
    {
        private class State<TState> : IDisposable
        {
            public void Dispose()
            {
                
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new State<TState>();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Console.WriteLine(formatter(state, exception));
        }
    }
}
