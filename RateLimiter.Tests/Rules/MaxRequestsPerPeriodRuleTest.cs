using System;
using System.Collections.Generic;
using NUnit.Framework;

using RateLimiter.Rules;

namespace RateLimiter.Tests.Rules
{
    [TestFixture]
    public class MaxRequestsPerPeriodRuleTest
    {
        [Test]
        public void FirstRequest()
        {
            //Arrange
            IRateLimiterRule rule = new MaxRequestsPerPeriodRule(1, TimeSpan.Zero);

            //Act
            var actual = rule.IsAllowed(Array.Empty<DateTimeOffset>(), DateTimeOffset.MinValue);

            //Assert
            Assert.True(actual);
        }

        [Test, TestCaseSource(nameof(RequestsOfUserData))]
        public void RequestsOfUser(int maxRequestsPerPeriod, TimeSpan period, TimeSpan requestDelay, int requestsCount, int expectedAllowedRequestsCount)
        {
            //Arrange
            var startRequestDateTime = DateTimeOffset.MinValue;
            var token = "Token";

            var history = new RequestsHistory();
            IRateLimiterRule rule = new MaxRequestsPerPeriodRule(maxRequestsPerPeriod, period);

            var actualAllowedRequestsCount = 0;

            //Act
            for (int i = 0; i < requestsCount; i++)
            {
                if (ProcessRequest(history, rule, token, startRequestDateTime))
                {
                    actualAllowedRequestsCount++;
                }

                startRequestDateTime = startRequestDateTime.Add(requestDelay);
            }

            //Assert
            Assert.AreEqual(expectedAllowedRequestsCount, actualAllowedRequestsCount);
        }

        [Test, TestCaseSource(nameof(RequestsOfNUsersData))]
        public void RequestsOfNUsers(int maxRequestsPerPeriod, TimeSpan minPeriod, TimeSpan requestDelay, int usersCount, int requestsCount, int expectedAllowedRequestsCount)
        {
            //Arrange
            DateTimeOffset[] requestsDateTime = new DateTimeOffset[usersCount];

            var history = new RequestsHistory();
            IRateLimiterRule rule = new MaxRequestsPerPeriodRule(maxRequestsPerPeriod, minPeriod);

            var actualAllowedRequestsCount = 0;

            //Act
            for (int i = 0; i < requestsCount; i++)
            {
                for (int u = 0; u < usersCount; u++)
                {
                    if (ProcessRequest(history, rule, u.ToString(), requestsDateTime[u]))
                    {
                        actualAllowedRequestsCount++;
                    }

                    requestsDateTime[u] = requestsDateTime[u].Add(requestDelay);
                }
            }

            //Assert
            Assert.AreEqual(expectedAllowedRequestsCount, actualAllowedRequestsCount);
        }

        private static bool ProcessRequest(RequestsHistory history, IRateLimiterRule rule, string token, DateTimeOffset requestDateTime)
        {
            var userRequests = history.GetRequests(token);

            var isAllowed = rule.IsAllowed(userRequests, requestDateTime);

            if (isAllowed) history.AcceptRequest(token, requestDateTime);

            return isAllowed;
        }

        public static IEnumerable<object[]> RequestsOfUserData()
        {
            return new List<object[]>
            {
                new object[] {2, TimeSpan.Zero, TimeSpan.Zero, 10, 10 },
                new object[] {2, TimeSpan.Zero, TimeSpan.Zero, 0, 0 },
                new object[] {2, TimeSpan.Zero, new TimeSpan(1), 10, 10 },
                new object[] {2, new TimeSpan(1), TimeSpan.Zero, 10, 2 },
                new object[] {2, new TimeSpan(2), new TimeSpan(1), 10, 10 },
                new object[] {2, new TimeSpan(4), new TimeSpan(1), 10, 6 },
                new object[] {1, new TimeSpan(2), new TimeSpan(1), 10, 5 }
            };
        }

        public static IEnumerable<object[]> RequestsOfNUsersData()
        {
            return new List<object[]>
            {
                new object[] {2, TimeSpan.Zero, TimeSpan.Zero, 5, 10, 50 },
                new object[] {2, TimeSpan.Zero, TimeSpan.Zero, 5, 0, 0 },
                new object[] {2, TimeSpan.Zero, new TimeSpan(1), 5, 10, 50 },
                new object[] {2, new TimeSpan(1), TimeSpan.Zero, 5, 10, 10 },
                new object[] {2, new TimeSpan(2), new TimeSpan(1), 5, 10, 50 },
                new object[] {2, new TimeSpan(4), new TimeSpan(1), 5, 10, 30 },
                new object[] {1, new TimeSpan(2), new TimeSpan(1), 5, 10, 25 }
            };
        }
    }
}