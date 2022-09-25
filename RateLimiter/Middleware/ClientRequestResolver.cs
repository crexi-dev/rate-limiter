using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using RateLimiter.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimiter.Middleware
{
    public class ClientRequestResolver : IClientRequestResolver
    {
        public async Task<ClientRequest> ResolveClientRequestAsync(HttpContext context)
        {
            var requestPath = context.Request.Path.ToString().ToLowerInvariant().TrimEnd('/');
            requestPath = string.IsNullOrWhiteSpace(requestPath) ? "/" : requestPath;
            var clientRequest = new ClientRequest
            {
                Resource = requestPath
            };

            var token = await context.GetTokenAsync("access_token") ?? context.Request.Headers[HeaderNames.Authorization].ToString()?.Replace("Bearer ", "") ?? null;

            if (string.IsNullOrWhiteSpace(token))
            {
                return clientRequest;
            }

            var handler = new JwtSecurityTokenHandler();
            var parsedToken = handler.ReadJwtToken(token);

            if (!parsedToken?.Claims?.Any() ?? true)
            {
                return clientRequest;
            }

            clientRequest.ClientKey = parsedToken.Claims.First(c => c.Type == "ClientKey")?.Value ?? null;
            clientRequest.Region = parsedToken.Claims.First(c => c.Type == "Region")?.Value ?? null;

            return clientRequest;
        }
    }
}
