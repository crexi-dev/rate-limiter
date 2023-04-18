using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using RateLimiter.Abstractions;

namespace RateLimiter;

public class RateLimitValidator : IRateLimitValidator
{
    private readonly IRateLimitService _rateLimitService;
    private readonly IRateLimitValidationQueryProvider _rateLimitValidationQueryProvider;
    private readonly IUserActivityRepository _userActivityRepository;

    public RateLimitValidator(IRateLimitService rateLimitService, IUserActivityRepository userActivityRepository,
        IRateLimitValidationQueryProvider rateLimitValidationQueryProvider)
    {
        _rateLimitService = rateLimitService;
        _userActivityRepository = userActivityRepository;
        _rateLimitValidationQueryProvider = rateLimitValidationQueryProvider;
    }

    public async Task ValidateAsync(string apiKey)
    {
        var rateLimits = await _rateLimitService.GetRateLimitsAsync(apiKey);
        var validationQuery = await _userActivityRepository.GetAllQueryableAsync(i => i.ApiKey == apiKey);

        foreach (var rateLimit in rateLimits)
            validationQuery = _rateLimitValidationQueryProvider.ApplyFilter(rateLimit, validationQuery);

        if (validationQuery.Any()) throw new ValidationException("Rate Limit Validation Failed");
    }
}