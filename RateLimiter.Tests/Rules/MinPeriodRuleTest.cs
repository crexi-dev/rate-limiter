using System;
using System.Collections.Generic;
using NUnit.Framework;

using RateLimiter.Rules;

namespace RateLimiter.Tests.Rules
{
    [TestFixture]
    public class MinPeriodRuleTest
    {
        [Test]
        public void FirstRequest()
        {
            //Arrange
            IRateLimiterRule rule = new MinPeriodRule(TimeSpan.Zero);

            //Act
            var actual = rule.IsAllowed(Array.Empty<DateTimeOffset>(), DateTimeOffset.MinValue);

            //Assert
            Assert.True(actual);
        }

        [Test, TestCaseSource(nameof(RequestsOfUserData))]
        public void RequestsOfUser(TimeSpan minPeriod, TimeSpan requestDelay, int requestsCount, int expectedAllowedRequestsCount)
        {
            //Arrange
            var requestDateTime = DateTimeOffset.MinValue;
            var token = "Token";

            var history = new RequestsHistory();
            IRateLimiterRule rule = new MinPeriodRule(minPeriod);

            var actualAllowedRequestsCount = 0;

            //Act
            for (int i = 0; i < requestsCount; i++)
            {
                if (ProcessRequest(history, rule, token, requestDateTime))
                {
                    actualAllowedRequestsCount++;
                }

                requestDateTime = requestDateTime.Add(requestDelay);
            }

            //Assert
            Assert.AreEqual(expectedAllowedRequestsCount, actualAllowedRequestsCount);
        }

        [Test, TestCaseSource(nameof(RequestsOfNUsersData))]
        public void RequestsOfNUsers(TimeSpan minPeriod, TimeSpan requestDelay, int usersCount, int requestsCount, int expectedAllowedRequestsCount)
        {
            //Arrange
            DateTimeOffset[] requestsDateTime = new DateTimeOffset[usersCount];

            var history = new RequestsHistory();
            IRateLimiterRule rule = new MinPeriodRule(minPeriod);

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
                new object[] { TimeSpan.Zero, TimeSpan.Zero, 10, 10 },
                new object[] { TimeSpan.Zero, TimeSpan.Zero, 0, 0 },
                new object[] { TimeSpan.Zero, new TimeSpan(1), 10, 10 },
                new object[] { new TimeSpan(1), TimeSpan.Zero, 10, 1 },
                new object[] { new TimeSpan(2), new TimeSpan(1), 10, 5 }
            };
        }

        public static IEnumerable<object[]> RequestsOfNUsersData()
        {
            return new List<object[]>
            {
                new object[] { TimeSpan.Zero, TimeSpan.Zero, 5, 10, 50 },
                new object[] { TimeSpan.Zero, TimeSpan.Zero, 5, 0, 0 },
                new object[] { TimeSpan.Zero, new TimeSpan(1), 5, 10, 50 },
                new object[] { new TimeSpan(1), TimeSpan.Zero, 5, 10, 5 },
                new object[] { new TimeSpan(2), new TimeSpan(1), 5, 10, 25 }
            };
        }
    }
}