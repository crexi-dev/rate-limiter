using Microsoft.VisualStudio.TestTools.UnitTesting;
using RateLimiter.DataAccess.Repositiories;
using System;
using System.Threading.Tasks;

namespace RateLimiter.Tests.DataAccess.Repositiories
{
    [TestClass]
    public class DistributedRateLimitRepositoryTests
    {
        [TestMethod, ExpectedException(typeof(NotImplementedException))]
        public async Task RetrieveTest()
        {
            var target = new DistributedRateLimitRepository();
            await target.Retrieve(string.Empty);
        }

        [TestMethod, ExpectedException(typeof(NotImplementedException))]
        public async Task UpdateTest()
        {
            var target = new DistributedRateLimitRepository();
            await target.Update(string.Empty, null);
        }
    }
}
