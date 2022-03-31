using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter
{
  public class AccessLog
  {
    public string Token { get; set; }
    public DateTime AccessDate { get; set; }
  }
}
