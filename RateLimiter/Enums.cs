using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter
{
  [Flags]
  public enum LimitTypes
  {
    PerTimeSpan = 1,
    SinceLastCall = 2,
    TokenOrigin = 4,
    TokenExpiration = 8
  }
}
