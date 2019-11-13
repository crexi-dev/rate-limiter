namespace RateLimiter.Tests
{
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;
    using static SessionState;
    using static SessionMessage;

    [TestFixture]
    public class RateLimiterTest
    {
        [Test]
        public void ClientWithNoTokenRateLimitTest()
        {
            Task<string> clientOneResult;
            var clientOne = new APIResourceController(null);
            clientOneResult = Task.Run(() => clientOne.GetUsersNames());
            Task.FromResult(clientOneResult).Result.Wait();
            clientOne.Dispose();
            Assert.IsTrue(Task.FromResult(clientOneResult).Result.Result == InvalidToken);
        }

        [Test]
        public void ClientOneWithOneResourceCallRateLimitTest()
        {
            List<Task<string>> clientOneResult = new List<Task<string>>();
            List<string> clientOneDataResult = new List<string>();
            int invokeCount = 7;
            var clientOne = new APIResourceController("Token1");
            for (int i = 0; i < invokeCount; i++)
            {
                clientOneResult.Add(Task.Run(() => clientOne.GetUsersNames()));
            }
            clientOneResult.ForEach(item =>
            {
                Task.FromResult(item).Result.Wait();
                clientOneDataResult.Add(Task.FromResult(item).Result.Result);
            });
            clientOne.Dispose();
            Assert.IsTrue(clientOneDataResult.Where(c => c != RateLimitExceeded).Count() == RateLimit);
        }

        [Test]
        public void ClientOneWithOneResourceCallDelayedRateLimitTest()
        {
            List<Task<string>> clientOneResult = new List<Task<string>>();
            List<string> clientOneDataResult = new List<string>();
            int invokeCount = 7;
            var clientOne = new APIResourceController("Token1");
            for (int i = 0; i < invokeCount; i++)
            {
                Task.Delay(2000).Wait();
                clientOneResult.Add(Task.Run(() => clientOne.GetUsersNames()));
            }
            clientOneResult.ForEach(item =>
            {
                Task.FromResult(item).Result.Wait();
                clientOneDataResult.Add(Task.FromResult(item).Result.Result);
            });
            clientOne.Dispose();
            Assert.IsTrue(clientOneDataResult.Where(c => c != RateLimitExceeded).Count() == invokeCount);
        }

        [Test]
        public void ClientOneWithTwoResourceCallRateLimitTest()
        {
            List<Task<string>> clientOneResult = new List<Task<string>>();
            List<string> clientOneDataResult = new List<string>();
            int invokeCount = 7;
            var clientOne = new APIResourceController("Token1");
            for (int i = 0; i < invokeCount; i++)
            {
                clientOneResult.Add(Task.Run(() => clientOne.GetUsers()));
                clientOneResult.Add(Task.Run(() => clientOne.GetUsersNames()));
            }
            clientOneResult.ForEach(item =>
            {
                Task.FromResult(item).Result.Wait();
                clientOneDataResult.Add(Task.FromResult(item).Result.Result);
            });
            clientOne.Dispose();
            Assert.IsTrue(clientOneDataResult.Where(c => c != RateLimitExceeded).Count() == RateLimit);
        }

        [Test]
        public void ClientOneWithTwoResourceCallDelayedRateLimitTest()
        {
            List<Task<string>> clientOneResult = new List<Task<string>>();
            List<string> clientOneDataResult = new List<string>();
            int invokeCount = 7;
            var clientOne = new APIResourceController("Token1");
            for (int i = 0; i < invokeCount; i++)
            {
                clientOneResult.Add(Task.Run(() => clientOne.GetUsers()));
                Task.Delay(2000).Wait();
                clientOneResult.Add(Task.Run(() => clientOne.GetUsersNames()));
            }
            clientOneResult.ForEach(item =>
            {
                Task.FromResult(item).Result.Wait();
                clientOneDataResult.Add(Task.FromResult(item).Result.Result);
            });
            clientOne.Dispose();
            Assert.IsTrue(clientOneDataResult.Where(c => c != RateLimitExceeded).Count() == invokeCount * 2);
        }

        [Test]
        public void MultipleClientsWithOneResourceCallRateLimitTest()
        {
            List<Task<string>> clientOneResult = new List<Task<string>>();
            List<Task<string>> clientTwoResult = new List<Task<string>>();
            List<string> clientOneDataResult = new List<string>();
            List<string> clientTwoDataResult = new List<string>();
            var clientOne = new APIResourceController("Token1");
            var clientTwo = new APIResourceController("Token2");
            int invokeCount = 7;
            for (int i = 0; i < invokeCount; i++)
            {
                clientOneResult.Add(Task.Run(() => clientOne.GetUsers()));
                clientTwoResult.Add(Task.Run(() => clientTwo.GetUsersNames()));
            }
            clientOneResult.ForEach(item =>
            {
                Task.FromResult(item).Result.Wait();
                clientOneDataResult.Add(Task.FromResult(item).Result.Result);
            });
            clientTwoResult.ForEach(item =>
            {
                Task.FromResult(item).Result.Wait();
                clientTwoDataResult.Add(Task.FromResult(item).Result.Result);
            });
            clientOne.Dispose();
            clientTwo.Dispose();
            Assert.IsTrue(clientTwoDataResult.Concat(clientOneDataResult).Where(c => c != RateLimitExceeded).Count() == RateLimit * 2);
        }

        [Test]
        public void MultipleClientsWithOneResourceDelayedCallRateLimitTest()
        {
            List<Task<string>> clientOneResult = new List<Task<string>>();
            List<Task<string>> clientTwoResult = new List<Task<string>>();
            List<string> clientOneDataResult = new List<string>();
            List<string> clientTwoDataResult = new List<string>();
            int invokeCount = 7;
            var clientOne = new APIResourceController("Token1");
            var clientTwo = new APIResourceController("Token2");
            for (int i = 0; i < invokeCount; i++)
            {
                clientOneResult.Add(Task.Run(() => clientOne.GetUsers()));
                Task.Delay(2000).Wait();
                clientTwoResult.Add(Task.Run(() => clientTwo.GetUsersNames()));
            }
            clientOneResult.ForEach(item =>
            {
                Task.FromResult(item).Result.Wait();
                clientOneDataResult.Add(Task.FromResult(item).Result.Result);
            });
            clientTwoResult.ForEach(item =>
            {
                Task.FromResult(item).Result.Wait();
                clientTwoDataResult.Add(Task.FromResult(item).Result.Result);
            });
            clientOne.Dispose();
            clientTwo.Dispose();
            Assert.IsTrue(clientTwoDataResult.Concat(clientOneDataResult).Where(c => c != RateLimitExceeded).Count() == invokeCount * 2);
        }

        [Test]
        public void MultipleClientsWithTwoResourceCallRateLimitTest()
        {
            List<Task<string>> clientOneResult = new List<Task<string>>();
            List<Task<string>> clientTwoResult = new List<Task<string>>();
            List<string> clientOneDataResult = new List<string>();
            List<string> clientTwoDataResult = new List<string>();
            int invokeCount = 7;
            var clientOne = new APIResourceController("Token1");
            var clientTwo = new APIResourceController("Token2");
            for (int i = 0; i < invokeCount; i++)
            {
                clientOneResult.Add(Task.Run(() => clientOne.GetUsers()));
                clientOneResult.Add(Task.Run(() => clientOne.GetUsersNames()));
                clientTwoResult.Add(Task.Run(() => clientTwo.GetUsers()));
                clientTwoResult.Add(Task.Run(() => clientTwo.GetUsersNames()));
            }
            clientOneResult.ForEach(item =>
            {
                Task.FromResult(item).Result.Wait();
                clientOneDataResult.Add(Task.FromResult(item).Result.Result);
            });
            clientTwoResult.ForEach(item =>
            {
                Task.FromResult(item).Result.Wait();
                clientTwoDataResult.Add(Task.FromResult(item).Result.Result);
            });
            clientOne.Dispose();
            clientTwo.Dispose();
            Assert.IsTrue(clientTwoDataResult.Concat(clientOneDataResult).Where(c => c != RateLimitExceeded).Count() == RateLimit * 2);
        }

        [Test]
        public void MultipleClientsWithTwoResourceDelayedCallRateLimitTest()
        {
            List<Task<string>> clientOneResult = new List<Task<string>>();
            List<Task<string>> clientTwoResult = new List<Task<string>>();
            List<string> clientOneDataResult = new List<string>();
            List<string> clientTwoDataResult = new List<string>();
            int invokeCount = 7;
            var clientOne = new APIResourceController("Token1");
            var clientTwo = new APIResourceController("Token2");
            for (int i = 0; i < invokeCount; i++)
            {
                clientOneResult.Add(Task.Run(() => clientOne.GetUsers()));
                clientOneResult.Add(Task.Run(() => clientOne.GetUsersNames()));
                Task.Delay(2000).Wait();
                clientTwoResult.Add(Task.Run(() => clientTwo.GetUsers()));
                clientTwoResult.Add(Task.Run(() => clientTwo.GetUsersNames()));
                Task.Delay(2000).Wait();
            }
            clientOneResult.ForEach(item =>
            {
                Task.FromResult(item).Result.Wait();
                clientOneDataResult.Add(Task.FromResult(item).Result.Result);
            });
            clientTwoResult.ForEach(item =>
            {
                Task.FromResult(item).Result.Wait();
                clientTwoDataResult.Add(Task.FromResult(item).Result.Result);
            });
            clientOne.Dispose();
            clientTwo.Dispose();
            Assert.IsTrue(clientTwoDataResult.Concat(clientOneDataResult).Where(c => c != RateLimitExceeded).Count() == invokeCount * 2 * 2);
        }
    }
}