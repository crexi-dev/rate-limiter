using Microsoft.VisualStudio.TestTools.UnitTesting;
using RateLimiter.DataAccess.Repositiories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Tests.DataAccess.Repositiories
{

    [TestClass]
    public class InMemoryRateLimitRepositoryTests
    {
        [TestMethod]
        public async Task RetrieveTest()
        {
            var parameters = new Dictionary<string, object>();

            var target = new InMemoryRateLimitRepository();

            var key = Guid.NewGuid().ToString();

            var accessor = new PrivateObject(target);

            var lookup = (ConcurrentDictionary<string, IDictionary<string, object>>)accessor.GetField("_lookup");

            lookup[key] = parameters;

            var actual = await target.Retrieve(key);

            Assert.AreSame(parameters, actual);
            Assert.IsNull(await target.Retrieve("123"));
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var key = Guid.NewGuid().ToString();

            var parameters = new Dictionary<string, object>();

            var target = new InMemoryRateLimitRepository();

            await target.Update(key, parameters);

            var actual = await target.Retrieve(key);

            Assert.AreSame(parameters, actual);
        }
    }
}
