using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter
{
  public class Resources
  {
    public bool ApiResource1(string token)
    {
      var limits = new Limits(maxCount: 3, secondsSinceLastCall: 10);
      return limits.Validate(LimitTypes.SinceLastCall | LimitTypes.PerTimeSpan, token);
    }
    public bool ApiResource2(string token)
    {
      var limits = new Limits();
      return limits.Validate(LimitTypes.TokenOrigin, token);
    }
    public bool ApiResource3(string token)
    {
      var limits = new Limits();
      return limits.Validate(LimitTypes.TokenOrigin | LimitTypes.TokenExpiration, token);
    }
  }
}
