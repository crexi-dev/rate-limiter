using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter
{
  public class ValidationMethod
  {
    public ValidationMethod(Func<string, Token, int, string, bool> method, int intValue, string stringValue)
    {
      this.Method = method;
      this.IntValue = intValue;
      this.StringValue = stringValue;
    }
    public Func<string, Token, int, string, bool> Method { get; set; }
    public int IntValue { get; set; }
    public string StringValue { get; set; }
  }
}
