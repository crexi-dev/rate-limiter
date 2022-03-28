using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RateLimiter.DAL;
using RateLimiter.Models;
using RateLimiter.Tests.Data;
using System;
using System.Collections.Generic;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        IRateLimit rateLimit;
        public RateLimiterTest()
        {
            var context = DefaultData.CreateContext();
            DateTime dateTime = new DateTime(2022, 01, 01, 12, 00, 00);

            rateLimit = new RateLimit(context, dateTime);

            rateLimit.SetRegions(DefaultData.GetRegions());
            rateLimit.SetResources(DefaultData.GetResources());
            rateLimit.SetRules(DefaultData.GetRules());
        }

        [Test]
        public void Rules_For_US_In_Timespan_Allowed_10SegsLater_For_Resource_CreateUser_Positive_Test()
        {
            //Arrange

            //----------------------
            //rateLimit arrange
            var existingRequests = DefaultData.GetExistingRequests(
                token: "ABC-0001",
                resourceId: DefaultData.GetResources()[0].Id,
                requestDate: new DateTime(2022, 01, 01, 12, 00, 00),
                requestedTimes: 5);

            rateLimit.SetExistingRequests(existingRequests);

            //bypassing the datetime when the request is made (10 segs later)
            rateLimit.SetDateTime(new DateTime(2022, 01, 01, 12, 00, 10));

            //----------------------
            //token arrange
            RateLimitToken token = new RateLimitToken();

            token.Token = "ABC-0001";
            token.ResourceId = DefaultData.GetResources()[0].Id;//CreateUser
            token.RegionId = DefaultData.GetRegions()[0].Id;//US

            //Act

            var result = rateLimit.ValidateToken(token);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Rules_For_US_In_Timespan_Allowed_5SegsLater_For_Resource_CreateUser_Positive_Test()
        {
            //Arrange

            //----------------------
            //rateLimit arrange
            var existingRequests = DefaultData.GetExistingRequests(
                token: "ABC-0001",
                resourceId: DefaultData.GetResources()[0].Id,
                requestDate: new DateTime(2022, 01, 01, 12, 00, 00),
                requestedTimes: 5);

            rateLimit.SetExistingRequests(existingRequests);

            //bypassing the datetime when the request is made (5 segs later)
            rateLimit.SetDateTime(new DateTime(2022, 01, 01, 12, 00, 05));

            //----------------------
            //token arrange
            RateLimitToken token = new RateLimitToken();

            token.Token = "ABC-0001";
            token.ResourceId = DefaultData.GetResources()[0].Id;//CreateUser
            token.RegionId = DefaultData.GetRegions()[0].Id;//US

            //Act

            var result = rateLimit.ValidateToken(token);

            //Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void Rules_For_US_In_0_Attempts_Allowed_For_Resource_CreateUser_Positive_Test()
        {
            //Arrange

            //----------------------
            //rateLimit arrange
            var existingRequests = DefaultData.GetExistingRequests(
                token: "ABC-0001",
                resourceId: DefaultData.GetResources()[0].Id,
                requestDate: new DateTime(2022, 01, 01, 12, 00, 00),
                requestedTimes: 0);

            rateLimit.SetExistingRequests(existingRequests);

            //bypassing the datetime when the request is made (5 segs later), used for rule 2
            rateLimit.SetDateTime(new DateTime(2022, 01, 01, 12, 00, 05));

            //----------------------
            //token arrange
            RateLimitToken token = new RateLimitToken();

            token.Token = "ABC-0001";
            token.ResourceId = DefaultData.GetResources()[0].Id;//CreateUser
            token.RegionId = DefaultData.GetRegions()[0].Id;//US

            //Act

            var result = rateLimit.ValidateToken(token);

            //Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void Rules_For_US_In_5_Attempts_Not_Allowed_For_Resource_CreateUser_Negative_Test()
        {
            //Arrange

            //----------------------
            //rateLimit arrange
            var existingRequests = DefaultData.GetExistingRequests(
                token: "ABC-0001",
                resourceId: DefaultData.GetResources()[0].Id,
                requestDate: new DateTime(2022, 01, 01, 12, 00, 00),
                requestedTimes: 5);

            rateLimit.SetExistingRequests(existingRequests);
            rateLimit.SetDateTime(new DateTime(2022, 01, 01, 12, 00, 00));

            //----------------------
            //token arrange
            RateLimitToken token = new RateLimitToken();

            token.Token = "ABC-0001";
            token.ResourceId = DefaultData.GetResources()[0].Id;//CreateUser
            token.RegionId = DefaultData.GetRegions()[0].Id;//US

            //Act

            var result = rateLimit.ValidateToken(token);

            //Assert
            Assert.IsFalse(result);
        }



        [Test]
        public void Rules_For_EU_In_Timespan_Allowed_59SegsLater_For_Resource_CreateUser_Positive_Test()
        {
            //Arrange

            //----------------------
            //rateLimit arrange
            var existingRequests = DefaultData.GetExistingRequests(
                token: "ABC-0001",
                resourceId: DefaultData.GetResources()[0].Id,
                requestDate: new DateTime(2022, 01, 01, 12, 00, 00),
                requestedTimes: 5);

            rateLimit.SetExistingRequests(existingRequests);

            //bypassing the datetime when the request is made (59 segs later)
            rateLimit.SetDateTime(new DateTime(2022, 01, 01, 12, 00, 59));

            //----------------------
            //token arrange
            RateLimitToken token = new RateLimitToken();

            token.Token = "ABC-0001";
            token.ResourceId = DefaultData.GetResources()[0].Id;//CreateUser
            token.RegionId = DefaultData.GetRegions()[1].Id;//US

            //Act

            var result = rateLimit.ValidateToken(token);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Rules_For_EU_In_Timespan_Allowed_50SegsLater_For_Resource_CreateUser_Positive_Test()
        {
            //Arrange

            //----------------------
            //rateLimit arrange
            var existingRequests = DefaultData.GetExistingRequests(
                token: "ABC-0001",
                resourceId: DefaultData.GetResources()[0].Id,
                requestDate: new DateTime(2022, 01, 01, 12, 00, 00),
                requestedTimes: 5);

            rateLimit.SetExistingRequests(existingRequests);

            //bypassing the datetime when the request is made (50 segs later)
            rateLimit.SetDateTime(new DateTime(2022, 01, 01, 12, 00, 50));

            //----------------------
            //token arrange
            RateLimitToken token = new RateLimitToken();

            token.Token = "ABC-0001";
            token.ResourceId = DefaultData.GetResources()[0].Id;//CreateUser
            token.RegionId = DefaultData.GetRegions()[1].Id;//US

            //Act

            var result = rateLimit.ValidateToken(token);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Rules_For_EU_In_0_Attempts_Allowed_For_Resource_CreateUser_Positive_Test()
        {
            //Arrange

            //----------------------
            //rateLimit arrange
            var existingRequests = DefaultData.GetExistingRequests(
                token: "ABC-0001",
                resourceId: DefaultData.GetResources()[0].Id,
                requestDate: new DateTime(2022, 01, 01, 12, 00, 00),
                requestedTimes: 0);

            rateLimit.SetExistingRequests(existingRequests);

            //bypassing the datetime when the request is made (50 segs later), used for rule 2
            rateLimit.SetDateTime(new DateTime(2022, 01, 01, 12, 00, 50));

            //----------------------
            //token arrange
            RateLimitToken token = new RateLimitToken();

            token.Token = "ABC-0001";
            token.ResourceId = DefaultData.GetResources()[0].Id;//CreateUser
            token.RegionId = DefaultData.GetRegions()[1].Id;//US

            //Act

            var result = rateLimit.ValidateToken(token);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Rules_For_EU_In_50_Attempts_Not_Allowed_For_Resource_CreateUser_Negative_Test()
        {
            //Arrange

            //----------------------
            //rateLimit arrange
            var existingRequests = DefaultData.GetExistingRequests(
                token: "ABC-0001",
                resourceId: DefaultData.GetResources()[0].Id,
                requestDate: new DateTime(2022, 01, 01, 12, 00, 00),
                requestedTimes: 50);

            rateLimit.SetExistingRequests(existingRequests);
            rateLimit.SetDateTime(new DateTime(2022, 01, 01, 12, 00, 00));
           //----------------------
           //token arrange
           RateLimitToken token = new RateLimitToken();

            token.Token = "ABC-0001";
            token.ResourceId = DefaultData.GetResources()[0].Id;//CreateUser
            token.RegionId = DefaultData.GetRegions()[1].Id;//US

            //Act

            var result = rateLimit.ValidateToken(token);

            //Assert
            Assert.IsFalse(result);
        }
    }
}
