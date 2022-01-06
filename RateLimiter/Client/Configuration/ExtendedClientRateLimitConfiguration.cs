using AspNetCoreRateLimit;
using Microsoft.Extensions.Options;

namespace RateLimiter.Client.Configuration
{
    /// <summary>
    /// Extended client configurator for cases, when client data must be retrieved on runtime, as usually, so we add extracting resolver
    /// </summary>
    public class ExtendedClientRateLimitConfiguration : RateLimitConfiguration
    {
        public ExtendedClientRateLimitConfiguration(IOptions<IpRateLimitOptions> ipOptions,
            IOptions<ClientRateLimitOptions> clientOptions) : base(ipOptions, clientOptions)
        {
        }

        public override void RegisterResolvers()
        {
            ClientResolvers.Add(new ExtendedClientResolveContributor());
            base.RegisterResolvers();
        }
    }
}
