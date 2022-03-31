using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Threading;

namespace RateLimiter.Tests
{
  [TestFixture]
  public class RateLimiterTest
  {
    string _euToken = Token.GenerateToken("EU");
    string _usToken = Token.GenerateToken("US");
    string _ruToken = Token.GenerateToken("RU");
    [Test]
    public void TestReesource1()
    {
      Resources resources = new Resources();
      Assert.IsTrue(resources.ApiResource1(_euToken));//EU token - Accepted at first call
      Thread.Sleep(5000);
      Assert.IsFalse(resources.ApiResource1(_euToken));//EU token - 5 seconds later EU token return false because service is configured to accept EU tokens only once in 10 seconds
      Thread.Sleep(5000);
      Assert.IsTrue(resources.ApiResource1(_euToken));//EU token - 10 seconds since first call and EU Token is Accepted again

      Assert.IsTrue(resources.ApiResource1(_usToken));//US Token - 1st request is accpeted, because service is configured to accpet 3 requests with US token in a past hour;
      Assert.IsTrue(resources.ApiResource1(_usToken));//US Token - 2nd request is accpeted, because service is configured to accpet 3 requests with US token in a past hour;
      Assert.IsTrue(resources.ApiResource1(_usToken));//US Token - 3rd request is accpeted, because service is configured to accpet 3 requests with US token in a past hour;
      Assert.IsFalse(resources.ApiResource1(_usToken));//US Token - 4th request return false, because service is configured to accpet 3 requests with US token in a past 
    }
    [Test]
    public void TestReesource2()
    {
      Resources resources = new Resources();
      Assert.IsFalse(resources.ApiResource2(_ruToken));//RU token - return false because resource is configured to accept tokens originated from US or EU
      Assert.IsTrue(resources.ApiResource2(_euToken));//EU token - return false because resource is configured to accept tokens originated from US or EU
      Assert.IsTrue(resources.ApiResource2(_usToken));//US token - return false because resource is configured to accept tokens originated from US or EU
    }
    [Test]
    public void TestReesource3()
    {
      var newUStoken = Token.GenerateToken("US");
      Resources resources = new Resources();
      Assert.IsTrue(resources.ApiResource3(newUStoken));//Token Accepted because token lifetime is set to 5 seconds
      Thread.Sleep(6000);
      Assert.IsFalse(resources.ApiResource3(newUStoken));//Token discarded because token lifetime is set to 5 seconds and more then 6 seconds has passed since token was issued
    }
  }
}
