using RateLimits.History;
using RateLimits.Rules;
using System;
using System.Collections.Generic;
using Xunit;

namespace RateLimits.Tests.Rules
{
    public class RequestsPerTimespanRuleTests
    {
        [Fact]
        public void AccessConfirmed()
        {
            RequestsPerTimespanRule rule = new RequestsPerTimespanRule(TimeSpan.FromSeconds(1), 5);
            var historyList = new List<HistoryModel>
            {
                new HistoryModel(DateTime.Now.AddMilliseconds(-10),"Tbilisi"),
                new HistoryModel(DateTime.Now.AddMilliseconds(-8),"Tbilisi"),
                new HistoryModel(DateTime.Now.AddMilliseconds(-7),"Tbilisi"),
                new HistoryModel(DateTime.Now.AddMilliseconds(-5),"Tbilisi")
            };
            var result = rule.Execute(historyList, "Tbilisi");
            Assert.True(result);
        }

        [Fact]
        public void AccessDenied()
        {
            RequestsPerTimespanRule rule = new RequestsPerTimespanRule(TimeSpan.FromSeconds(1), 5);
            var historyList = new List<HistoryModel>
            {
                new HistoryModel(DateTime.Now.AddMilliseconds(-10),"Tbilisi"),
                new HistoryModel(DateTime.Now.AddMilliseconds(-8),"Tbilisi"),
                new HistoryModel(DateTime.Now.AddMilliseconds(-7),"Tbilisi"),
                new HistoryModel(DateTime.Now.AddMilliseconds(-5),"Tbilisi"),
                new HistoryModel(DateTime.Now.AddMilliseconds(-1),"Tbilisi")
            };
            var result = rule.Execute(historyList, "Tbilisi");
            Assert.False(result);
        }
    }
}
