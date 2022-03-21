using RateLimiter.Core.Models.Identity;

namespace RateLimiter.API.Resolvers;

public interface IIdentityResolver
{
    ClientRequestIdentity ResolveClient(HttpContext context);

    string Encrypt(string text);
    string Decrypt(string encrypted);
}