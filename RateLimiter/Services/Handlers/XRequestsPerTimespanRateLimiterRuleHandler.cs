using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RateLimiter.Services.Handlers.Models;
using RateLimiter.Services.Handlers.Validators;
using RateLimiter.UtilityServices;

namespace RateLimiter.Services.Handlers
{
    public class XRequestsPerTimespanRateLimiterRuleHandler : IRequestHandler<XRequestsPerTimespanRateLimiterRuleHandlerModel, bool>
    {
        private readonly IBaseHandlerModelValidator _validator;
        private readonly IDateTimeProvider _dateTimeProvider;

        public XRequestsPerTimespanRateLimiterRuleHandler(IBaseHandlerModelValidator validator,
            IDateTimeProvider dateTimeProvider)
        {
            _validator = validator;
            _dateTimeProvider = dateTimeProvider;
        }

        public Task<bool> Handle(XRequestsPerTimespanRateLimiterRuleHandlerModel request, CancellationToken cancellationToken)
        {
            _validator.Validate(request);
            int requestNumbers = 0;
            for (int i = request.UserInformation.RequestEntries.Count - 1; i >= 0; i--)
            {
                var timeSpanIsPassedSinceCurrentRequestEntry = _dateTimeProvider.GetDateTimeUtcNow() - request.UserInformation.RequestEntries[i];
                if (timeSpanIsPassedSinceCurrentRequestEntry >= request.Rule.TimeSpanPeriod)
                {
                    break;
                }

                ++requestNumbers;
            }

            var isPassed = requestNumbers < request.Rule.RequestsLimit;

            return Task.FromResult(isPassed);
        }
    }
}
