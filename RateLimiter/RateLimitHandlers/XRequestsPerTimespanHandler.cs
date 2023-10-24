using Microsoft.Extensions.Options;
using RateLimiter.Contexts;
using RateLimiter.Enums;
using RateLimiter.Models;
using RateLimiter.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.RateLimitHandlers
{
    public class XRequestsPerTimespanHandler : RateLimitHandlerBase<IClientRequestIdentity, QuotaExceededResponse> //must be the IQuotaResponse
    {
        private readonly ICacheService _cacheService;
        private readonly IOptions<RateLimitOptions> _options;

        public override ERoleType RoleType { get; init; }

        public XRequestsPerTimespanHandler(ICacheService cacheService, IOptions<RateLimitOptions> options)
        {
            RoleType = ERoleType.XRequestsPerTimespan;
            _cacheService = cacheService;
            _options = options;
        }

        public override async Task<QuotaExceededResponse> CheckLimit(IClientRequestIdentity client)
        {
            //Options values can be passed already filtered by client
            var clientOptions = _options.Value.ClientOptions.Where(x => x.ClientId == client.ClientId && x.RoleType.HasFlag(RoleType))
                                                            .ToList();

            foreach (var item in clientOptions)
            {
                var lastClientCall = await _cacheService.GetClientLastRecord(item.ClientId);

                if (lastClientCall == null)
                    return await ToNext(client);

                var count = await _cacheService.GetRecordsCount<RequestHistoryEModel>(x => x.ClientId == client.ClientId && x.ReqDate >= DateTime.UtcNow.AddSeconds(-item.Period));
                if (count < item.Limit)
                    return await ToNext(client);

                return new QuotaExceededResponse()
                {
                    Message = $"API calls quota exceeded! maximum admitted {item.Limit} request per {item.Period} second(s).",
                    RetryAfter = $"Few seconds"
                };
            }

            return await ToNext(client);
        }
    }
}
