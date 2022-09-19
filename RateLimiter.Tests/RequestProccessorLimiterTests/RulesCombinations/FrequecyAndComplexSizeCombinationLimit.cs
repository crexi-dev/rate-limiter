using NUnit.Framework;
using RateLimiter.Rules;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Tests.RequestProccessorLimiterTests.RulesCombinations
{
    /// <summary>
    /// A demo to use two rules. Request frequncy and odd-even balance control
    /// </summary>
    public class FrequecyAndComplexSizeCombinationLimit
    {
        private ILimiter testObject;

        [SetUp]
        public void TestInit()
        {
            testObject = new RequestProccessorLimiter(new RequestProcessor(), new List<ILimiteRule>());
            testObject.AddRule(new Rules.RequestFrequencyLimiteRule(50));
            testObject.AddRule(new OddOrEvenSizeByPeriodLimiteRule(1000, 1000));
        }

        [Test, Timeout(60*1000)]
        public void Run100WithSize50_With20Success()
        {
            List<Task<Response>> tasks = new List<Task<Response>>();

            for (int i = 0; i < 100; i++)
            {
                tasks.Add(testObject.DoRequestAsync(new Request() { Size = 500 + i }));
                Thread.Sleep(i * 5);
            }

            Task.WaitAll(tasks.ToArray());

            var successCount = tasks.Select(t => t.Result).Where(r => r.IsSuccessful).Count();
            var failedCount = tasks.Select(t => t.Result).Where(r => !r.IsSuccessful).Count();

            Assert.AreEqual(35, successCount); // Checked in Excel
        }
    }
}
