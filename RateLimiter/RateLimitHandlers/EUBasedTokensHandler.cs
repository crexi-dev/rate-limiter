using Microsoft.Extensions.Options;
using RateLimiter.Enums;
using RateLimiter.Models;
using RateLimiter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.RateLimitHandlers
{
    public class EUBasedTokensHandler : RateLimitHandlerBase<IClientRequestIdentity, QuotaExceededResponse> //must be the IQuotaResponse
    {
        private readonly ICacheService _cacheService;
        private readonly IOptions<RateLimitOptions> _options;

        public override ERoleType RoleType { get; init; }

        public EUBasedTokensHandler(ICacheService cacheService, IOptions<RateLimitOptions> options)
        {
            RoleType = ERoleType.EUBasedTokens;
            _cacheService = cacheService;
            _options = options;
        }

        public override async Task<QuotaExceededResponse> CheckLimit(IClientRequestIdentity client)
        {
            if (client.RegionPrefix.ToLower() != "eu")
                return await ToNext(client);

            //Options values can be passed already filtered by client
            var clientOptions = _options.Value.ClientOptions.Where(x => x.ClientId == client.ClientId && x.RoleType.HasFlag(RoleType))
                                                            .ToList();

            foreach (var item in clientOptions)
            {
                var lastClientCall = await _cacheService.GetClientLastRecord(item.ClientId);

                if (lastClientCall == null)
                    return await ToNext(client);

                var span = DateTime.UtcNow - lastClientCall.ReqDate;
                if (span.Seconds >= item.Period)
                    return await ToNext(client);

                return new QuotaExceededResponse()
                {
                    Message = $"API calls quota exceeded! maximum admitted 1 request per {item.Period} second(s).",
                    RetryAfter = $"{item.Period - span.Seconds} seconds"
                };
            }

            return await ToNext(client);
        }
    }
}
