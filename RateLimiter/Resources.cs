using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter
{
  public class Resources
  {
    public bool ApiResource1(string token)
    {
      var validationMethods = new List<ValidationMethod>();
      validationMethods.Add(new ValidationMethod(Limits.SinceLastCall, 10, "US"));
      validationMethods.Add(new ValidationMethod(Limits.PerTimeSpan, 3, "EU"));

      return Limits.Validate(token, validationMethods);
    }
    public bool ApiResource2(string token)
    {
      //var limits = new Limits();
      //return limits.Validate(LimitTypes.TokenOrigin, token);
      var validationMethods = new List<ValidationMethod>();
      validationMethods.Add(new ValidationMethod(Limits.TokenOrigin, 0, "EU,US"));
      validationMethods.Add(new ValidationMethod(
        (string token, Token tokenData, int tokenLength, string stringVal) =>
        {
          return token.Length > tokenLength;
        },
        5, null));

      return Limits.Validate(token, validationMethods);
    }
    public bool ApiResource3(string token)
    {
      //var limits = new Limits();
      //return limits.Validate(LimitTypes.TokenOrigin | LimitTypes.TokenExpiration, token);

      var validationMethods = new List<ValidationMethod>();
      validationMethods.Add(new ValidationMethod(Limits.TokenOrigin, 0, "EU,US"));
      validationMethods.Add(new ValidationMethod(Limits.TokenExpiration, 5, null));

      return Limits.Validate(token, validationMethods);
    }
  }
}
