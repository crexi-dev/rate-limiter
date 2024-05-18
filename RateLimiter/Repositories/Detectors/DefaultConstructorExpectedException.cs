using System;
using System.Runtime.Serialization;

namespace RateLimiter.Repositories.Detectors
{
    [Serializable]
    internal class DefaultConstructorExpectedException : Exception
    {
        public DefaultConstructorExpectedException()
        {
        }

        public DefaultConstructorExpectedException(string? message) : base(message)
        {
        }

        public DefaultConstructorExpectedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected DefaultConstructorExpectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}