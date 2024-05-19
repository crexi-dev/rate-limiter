using System;
using System.Runtime.Serialization;

namespace RateLimiter.RuleTemplates.RequestConverters
{
    [Serializable]
    internal class InvalidRuleTemplateParamsException : Exception
    {
        public InvalidRuleTemplateParamsException()
        {
        }

        public InvalidRuleTemplateParamsException(string? message) : base(message)
        {
        }

        public InvalidRuleTemplateParamsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidRuleTemplateParamsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}