using RateLimiter.Common;
using RateLimiter.Domain;
using RateLimiter.RateLimiter.Rules;
using RateLimiter.Storage;
using System.Threading.Tasks;

namespace RateLimiter.RateLimiter.Services
{
    public sealed class RateLimitService : IRateLimitService
    {
        private readonly IUserRequestRepository userRequestRepository;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly IRateLimitPolicy rateLimitPolicy;

        public RateLimitService(
            IUserRequestRepository userRequestRepository,
            IDateTimeProvider dateTimeProvider,
            IRateLimitPolicy rateLimitPolicy)
        {
            this.userRequestRepository = userRequestRepository;
            this.dateTimeProvider = dateTimeProvider;
            this.rateLimitPolicy = rateLimitPolicy;
        }

        public async Task<bool> ValidateAsync(string accessToken, string resourceName)
        {
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(resourceName))
                return true;

            var userRequests = await userRequestRepository.GetAllAsync(accessToken);
            var currentDate = dateTimeProvider.GetUtcDate();

            var isValid = rateLimitPolicy.Check(accessToken, userRequests, currentDate);

            if (!isValid)
                return false;

            var userRequest = new UserRequest
            {
                AccessToken = accessToken,
                Date = currentDate,
                ResourceName = resourceName
            };

            await userRequestRepository.AddOrUpdateAsync(userRequest);

            return true;
        }
    }
}
