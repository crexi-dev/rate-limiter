using NUnit.Framework;
using RateLimiter.Common;
using RateLimiter.Services;
using System.Threading;

namespace RateLimiter.Tests
{


    [TestFixture]
    public class RateLimiterTest
    {
        [Test]
        public void NotValidCustomer()
        {
            ApiService service = new ApiService(new ApiDataService());
            Assert.IsFalse(service.NewRequest("Client C", new CustomerRequest { CustomerId = 1, Request = "New space in irvine" }));
        }


        [Test]
        public void ValidUkRequest()
        {
            ApiService service = new ApiService(new ApiDataService());
            Thread.Sleep(3000);
            Assert.IsTrue(service.NewRequest("Client B", new CustomerRequest { CustomerId = 2, Request = "New space in irvine" }));
        }

        [Test]
        public void RegionUkTimeoutValid()
        {
            ApiService service = new ApiService(new ApiDataService());
            Thread.Sleep(3000);
            Assert.IsTrue(service.NewRequest("Client B", new CustomerRequest { CustomerId = 2, Request = "New space in irvine" }));
            Assert.IsFalse(service.NewRequest("Client B", new CustomerRequest { CustomerId = 2, Request = "New space in irvine" }));
        }

        [Test]
        public void ValidUsRequest()
        {
            ApiService service = new ApiService(new ApiDataService());
            Assert.IsTrue(service.NewRequest("Client A", new CustomerRequest { CustomerId = 1, Request = "New space in irvine" }));
        }

        [Test]
        public void MaxUsRequest()
        {
            //Test data has 2 request
            ApiService service = new ApiService(new ApiDataService());
            //Adding 3 request
            Assert.IsTrue(service.NewRequest("Client A", new CustomerRequest { CustomerId = 1, Request = "New space in irvine" }));
            //Max 3 allowed adding 4th
            Assert.IsFalse(service.NewRequest("Client A", new CustomerRequest { CustomerId = 1, Request = "New space in irvine" }));
        }
    }
}
