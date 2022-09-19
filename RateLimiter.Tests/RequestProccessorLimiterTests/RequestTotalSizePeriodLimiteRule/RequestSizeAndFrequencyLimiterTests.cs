using NUnit.Framework;
using RateLimiter.Rules;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Tests.RequestProccessorLimiterTests.RequestTotalSizePeriodLimiteRule
{
    public class RequestSizeAndFrequencyLimiterTests
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
            public void Run100WithSize50_With20Success()
            {
                testObject.AddRule(new Rules.RequestTotalSizePeriodLimiteRule(1000, 1000));

                List<Task<Response>> tasks = new List<Task<Response>>();

                for (int i = 0; i < 100; i++)
                {
                    tasks.Add(testObject.DoRequestAsync(new Request() { Size = 50 }));
                }

                Task.WaitAll(tasks.ToArray());


                var successCount = tasks.Select(t => t.Result).Where(r => r.IsSuccessful).Count();
                var failedCount = tasks.Select(t => t.Result).Where(r => !r.IsSuccessful).Count();

                Assert.AreEqual(20, successCount);
                Assert.AreEqual(80, failedCount);
            }

            [Test]
            public void Run50PlusDelayPlus50_With40Success()
            {
                testObject.AddRule(new Rules.RequestTotalSizePeriodLimiteRule(1000, 1000));

                List<Task<Response>> tasks = new List<Task<Response>>();

                for (int i = 0; i < 50; i++)
                {
                    tasks.Add(testObject.DoRequestAsync(new Request() { Size = 50 }));
                }

                Thread.Sleep(1000);

                for (int i = 0; i < 50; i++)
                {
                    tasks.Add(testObject.DoRequestAsync(new Request() { Size = 50 }));
                }

                Task.WaitAll(tasks.ToArray());


                var successCount = tasks.Select(t => t.Result).Where(r => r.IsSuccessful).Count();
                var failedCount = tasks.Select(t => t.Result).Where(r => !r.IsSuccessful).Count();

                Assert.AreEqual(40, successCount);
            }
        }
    }
}
