using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace RateLimiter;

public static class Helpers
{
    public static Option? GetDefault(List<Option> options)
    {
        var defaultOption = options.Where(x => x.Type?.ToLower() == "default").FirstOrDefault();

        return defaultOption;
    }

    public static Option? GetRegionOption(List<Option> options, UserSettings user)
    {
        var regionSettings = options.Where(x => x.Region?.ToLower() == user.Region.ToString().ToLower()).FirstOrDefault();

        return regionSettings;
    }

    public static Option? GetTierOption(List<Option> options, UserSettings user)
    {
        var tierSettings = options.Where(x => x.Tier?.ToLower() == user.ServiceTier.ToString().ToLower()).FirstOrDefault();

        return tierSettings;
    }
}