using NUnit.Framework;
using RateLimiter.Rules;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Tests.RequestProccessorLimiterTests.RequestFrequencyLimiteRule
{
    [TestFixture]
    public class FrequencyLimiterTests
    {
        private ILimiter testObject;

        [SetUp]
        public void TestInit()
        {
            testObject = new RequestProccessorLimiter(new RequestProcessor(), new List<ILimiteRule>());
            
        }

        [Test]
        public void Run100Immediatly_WithOneSuccess()
        {
            testObject.AddRule(new Rules.RequestFrequencyLimiteRule(100));

            List<Task<Response>> tasks = new List<Task<Response>>();    

            for(int i = 0; i < 100; i++)
            {
                tasks.Add(testObject.DoRequestAsync(new Request() { ProcessDurationMs = 50 }));
            }

            Task.WaitAll(tasks.ToArray());


            var successCount = tasks.Select(t => t.Result).Where(r => r.IsSuccessful).Count();
            var failedCount = tasks.Select(t => t.Result).Where(r => !r.IsSuccessful).Count();

            Assert.AreEqual(1, successCount);
            Assert.AreEqual(99, failedCount);
        }

        [Test]
        public void Run50PlusDelayPlus50_WithTwoSuccess()
        {
            testObject.AddRule(new Rules.RequestFrequencyLimiteRule(100));

            List<Task<Response>> tasks = new List<Task<Response>>();

            for (int i = 0; i < 50; i++)
            {
                tasks.Add(testObject.DoRequestAsync(new Request() { ProcessDurationMs = 50 }));
            }

            Thread.Sleep(100);

            for (int i = 0; i < 50; i++)
            {
                tasks.Add(testObject.DoRequestAsync(new Request() { ProcessDurationMs = 50 }));
            }

            Task.WaitAll(tasks.ToArray());


            var successCount = tasks.Select(t => t.Result).Where(r => r.IsSuccessful).Count();
            var failedCount = tasks.Select(t => t.Result).Where(r => !r.IsSuccessful).Count();

            Assert.AreEqual(2, successCount);
            Assert.AreEqual(98, failedCount);
        }

    }
}
