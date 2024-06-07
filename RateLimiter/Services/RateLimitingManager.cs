using System;
using System.Threading.Tasks;
using RateLimiter.Extension;
using RateLimiter.Models;
using RateLimiter.Repositories.Interfaces;
using RateLimiter.Services.Interfaces;

namespace RateLimiter.Services;

public class RateLimitingManager(IRequestValidator firstValidator, IRateLimitRepository rateLimitRepository)
    : IRateLimitingManager
{
    public async Task<(int? StatusCode, string Message)> IsRequestAllowedAsync(string resource, string accessToken)
    {
        var region = accessToken.GetRegionFromToken();
        var requestHistory = await rateLimitRepository.GetAsync($"{accessToken}_{resource}");
        var (isAllowed, statusCode, message) = firstValidator.Check(resource, region, requestHistory);

        await SaveRequestAsync(resource, accessToken, region, statusCode);

        return (statusCode, message);
    }

    private async Task SaveRequestAsync(string resource, string accessToken, Region? region, int? statusCode)
    {
        var model = new RateLimitRequestModel
        {
            AccessToken = accessToken,
            DateTime = DateTime.UtcNow,
            Path = resource,
            Region = region,
            StatusCode = statusCode
        };

        await rateLimitRepository.AddAsync(model);
    }
}