using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter
{
  public class Token
  {
    public string Origin { get; set; }
    public string Iat { get; set; }

    public static string GenerateToken(string origin)
    {
      return JsonConvert.SerializeObject(new Token() { Origin = origin, Iat = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds().ToString() });
    }
    public static Token ReadTokenData(string token)
    {
      if (String.IsNullOrEmpty(token)) throw new Exception("Invalid Token");
      var result = JsonConvert.DeserializeObject<Token>(token);
      if (result == null) throw new Exception("Invalid Token");
      return result;
    }
  }
}
