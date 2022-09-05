using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Tokenizer
{
    // This should be a service
    public static class AuthHelpers
    {
        private const string tokenSecret = "This is a very, very dark secret"; // should be from appsettings
        private const string claimType_ClientId = "clientId";
        private const string claimType_Region = "region";

        public static string CreateToken(string clientId, string region)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                claims: new List<Claim>
                {
                    new Claim(claimType_ClientId, clientId),
                    new Claim(claimType_Region, region)
                },
                signingCredentials: creds,
                expires: DateTime.UtcNow.AddMinutes(15));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static TokenClaims GetTokenClaims(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var clientId = jwtSecurityToken.Claims.First(claim => claim.Type == claimType_ClientId).Value;
            var region = jwtSecurityToken.Claims.First(claim => claim.Type == claimType_Region).Value;

            return new TokenClaims(clientId, region);

        }

        //protected string GetName(string token)
        //{
        //    string secret = "this is a string used for encrypt and decrypt token";
        //    var key = Encoding.ASCII.GetBytes(secret);
        //    var handler = new JwtSecurityTokenHandler();
        //    var validations = new TokenValidationParameters
        //    {
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(key),
        //        ValidateIssuer = false,
        //        ValidateAudience = false
        //    };
        //    var claims = handler.ValidateToken(token, validations, out var tokenSecure);
        //    return claims.Identity.Name;
        //}
    }
}
