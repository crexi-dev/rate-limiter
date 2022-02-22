using RateLimits.History;
using RateLimits.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RateLimits.Tests.Rules
{
    public class TimespanSinceLastCallRuleTests
    {
        [Fact]
        public void AccessConfirmed()
        {
            TimespanSinceLastCallRule rule = new TimespanSinceLastCallRule(TimeSpan.FromSeconds(3));
            var historyList = new List<HistoryModel>
            {
                new HistoryModel(DateTime.Now.AddSeconds(-5),"Tbilisi")
            };
            var result = rule.Execute(historyList, "Tbilisi");
            Assert.True(result);
        }

        [Fact]
        public void AccessDenied()
        {
            TimespanSinceLastCallRule rule = new TimespanSinceLastCallRule(TimeSpan.FromSeconds(3));
            var historyList = new List<HistoryModel>
            {
                new HistoryModel(DateTime.Now.AddSeconds(-1),"Tbilisi")
            };
            var result = rule.Execute(historyList, "Tbilisi");
            Assert.False(result);
        }
    }
}
