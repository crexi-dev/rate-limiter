using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RateLimiter
{
  public class Limits
  {
    #region ***Private
    private static List<AccessLog> _accessHistory = new List<AccessLog>();
    #endregion


    #region ***Public
    public static bool Validate(string token, List<ValidationMethod> validationMethods)
    {
      Token tokenData = Token.ReadTokenData(token);
      bool result = true;
      foreach (var validationMethod in validationMethods)
      {
        result = result & validationMethod.Method(token, tokenData, validationMethod.IntValue, validationMethod.StringValue);
      }

      if (result)
      {
        _accessHistory.Add(new AccessLog() { Token = token, AccessDate = DateTime.Now });
      }
      return result;
    }
    #endregion

    #region ***ValidationTypes
    public static bool TokenExpiration(string token, Token tokenData, int tokenLifetimeSeconds, string stringVlue)
    {
      double tokenLifetime = (double)tokenLifetimeSeconds;
      var checkValues = DateTimeOffset.UtcNow.AddSeconds(-tokenLifetime).ToUnixTimeMilliseconds();
      return long.Parse(tokenData.Iat) > checkValues;
    }

    public static bool TokenOrigin(string token, Token tokenData, int intValue, string origins)
    {
      var allowedOrigins = origins.Split(",");
      return allowedOrigins.Contains(tokenData.Origin);
    }

    public static bool PerTimeSpan(string token, Token tokenData, int maxCount, string skipOrigins)
    {
      if (skipOrigins.Split(",").Contains(tokenData.Origin))
        return true;
      var AccessCount = _accessHistory.Where(acc => acc.Token == token && acc.AccessDate > DateTime.Now.AddHours(-1));
      return AccessCount.Count() < maxCount;
    }
    public static bool SinceLastCall(string token, Token tokenData, int secondsSinceLastCall, string skipOrigins)
    {
      if (skipOrigins.Split(",").Contains(tokenData.Origin))
        return true;
      var tokenHistory = _accessHistory.Where(acc => acc.Token == token);
      if (!tokenHistory.Any()) return true;
      var lastCallDate = tokenHistory.Max(acc => acc.AccessDate);
      return lastCallDate.AddSeconds(secondsSinceLastCall) < DateTime.Now;
    }
    #endregion

  }

}
