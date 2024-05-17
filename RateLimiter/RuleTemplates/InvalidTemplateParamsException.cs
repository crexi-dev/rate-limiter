using System;
using System.Runtime.Serialization;

namespace RateLimiter.RuleTemplates
{
    [Serializable]
    internal class InvalidTemplateParamsException : Exception
    {
        public InvalidTemplateParamsException()
        {
        }

        public InvalidTemplateParamsException(string? message) : base(message)
        {
        }

        public InvalidTemplateParamsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidTemplateParamsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}