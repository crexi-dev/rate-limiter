using NUnit.Framework;
using RateLimiter.Repositories;
using System;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        [Test]
        public void Example()
        {
            OrderRepository order= new OrderRepository();
            for (int i = 1; i < 3; i++)
            {
                if (i == 2)
                {
                    try
                    {
                        order.GetAll();
                    }
                    catch (Exception ex)
                    {
                        Assert.AreEqual(ex.Message, "the request limit is expired!!!");
                    }
                }
                else
                {
                    order.GetAll();
                }
            }
        }
    }
}
