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
    private int _maxCount;
    private int _secondsSinceLastCall;
    private double _tokenLifetimeMs;
    private static List<AccessLog> _accessHistory = new List<AccessLog>();
    private static List<string> _AllowedOrigins = new List<string>() { "EU", "US" };
    #endregion
    #region ***Ctor
    public Limits(int maxCount = 1000, int secondsSinceLastCall = 1000, double tokenLifeTimeMs = 5)
    {
      _maxCount = maxCount;
      _secondsSinceLastCall = secondsSinceLastCall;
      _tokenLifetimeMs = tokenLifeTimeMs;
    }
    #endregion

    #region ***Public
    public bool Validate(LimitTypes limittype, string token)
    {
      Token tokenData = Token.ReadTokenData(token);
      bool result = true;

      if ((limittype & LimitTypes.PerTimeSpan) == LimitTypes.PerTimeSpan && tokenData.Origin != "EU")
      {
        result = result & PerTimeSpan(token);
      }
      if ((limittype & LimitTypes.SinceLastCall) == LimitTypes.SinceLastCall && tokenData.Origin != "US")
      {
        result = result & SinceLastCall(token);
      }
      if ((limittype & LimitTypes.TokenOrigin) == LimitTypes.TokenOrigin)
      {
        result = result & TokenOrigin(tokenData);
      }
      if ((limittype & LimitTypes.TokenExpiration) == LimitTypes.TokenExpiration)
      {
        result = result & TokenExpiration(tokenData);
      }
      if (result)
      {
        _accessHistory.Add(new AccessLog() { Token = token, AccessDate = DateTime.Now });
      }
      return result;
    }
    #endregion

    #region ***Private
    private bool TokenExpiration(Token tokenData)
    {
      var checkValues = DateTimeOffset.UtcNow.AddSeconds(-_tokenLifetimeMs).ToUnixTimeMilliseconds();
      return long.Parse(tokenData.Iat) > checkValues;
    }

    private bool TokenOrigin(Token tokenData)
    {
      return _AllowedOrigins.Contains(tokenData.Origin);
    }

    private bool PerTimeSpan(string token)
    {

      var AccessCount = _accessHistory.Where(acc => acc.Token == token && acc.AccessDate > DateTime.Now.AddHours(-1));
      return AccessCount.Count() < _maxCount;
    }
    private bool SinceLastCall(string token)
    {
      var tokenHistory = _accessHistory.Where(acc => acc.Token == token);
      if (!tokenHistory.Any()) return true;
      var lastCallDate = tokenHistory.Max(acc => acc.AccessDate);
      return lastCallDate.AddSeconds(_secondsSinceLastCall) < DateTime.Now;
    }
    #endregion

  }
}
