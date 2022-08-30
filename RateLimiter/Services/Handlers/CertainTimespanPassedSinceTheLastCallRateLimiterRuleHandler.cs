using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RateLimiter.Services.Handlers.Models;
using RateLimiter.Services.Handlers.Validators;
using RateLimiter.UtilityServices;

namespace RateLimiter.Services.Handlers
{
    public class CertainTimespanPassedSinceTheLastCallRateLimiterRuleHandler : IRequestHandler<CertainTimespanPassedSinceTheLastCallRateLimiterRuleHandlerModel, bool>
    {
        private readonly IBaseHandlerModelValidator _validator;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CertainTimespanPassedSinceTheLastCallRateLimiterRuleHandler(IBaseHandlerModelValidator validator,
            IDateTimeProvider dateTimeProvider)
        {
            _validator = validator;
            _dateTimeProvider = dateTimeProvider;
        }

        public Task<bool> Handle(CertainTimespanPassedSinceTheLastCallRateLimiterRuleHandlerModel request,
            CancellationToken cancellationToken)
        {
            _validator.Validate(request);
            var timeSpanPassed = _dateTimeProvider.GetDateTimeUtcNow() - request.UserInformation.RequestEntries.Last();
            var isPassed = timeSpanPassed >= request.Rule.TimeSpanPeriod;

            return Task.FromResult(isPassed);
        }
    }
}
