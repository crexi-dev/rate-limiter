using System.Diagnostics.CodeAnalysis;
using MediatR;
using RateLimiter.Models;
using RateLimiter.Models.Rules;

namespace RateLimiter.Services.Handlers.Models
{
    [ExcludeFromCodeCoverage]
    public class BaseHandlerModel<TRule> : IRequest<bool>
        where TRule : RateLimiterRuleBase
    {
        public string Token { get; set; }
        public UserInformation UserInformation { get; set; }
        public TRule Rule { get; set; }
    }
}
