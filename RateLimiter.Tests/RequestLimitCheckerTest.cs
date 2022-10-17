using Microsoft.VisualStudio.TestTools.UnitTesting;
using RateLimiter.DataAccess.Query;
using RateLimiter.DataModel;
using RateLimiter.Validators;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RateLimiter.Tests
{
    [TestClass]
    public class RequestLimitCheckerTest
    {
        IQueryRepo _queryRepo;
        IRuleValidatorFactory _ruleValidatoryFactory;
        IRuleValidator _ruleValidator1, _ruleValidator2;
        IRequestLimitChecker _limitChecker;

        [TestInitialize]
        public void Initialize()
        {
            _queryRepo = new QueryRepo();
            _ruleValidator1 = new RequestCountValidator();
            _ruleValidator2 = new TimespanValidator();
            _ruleValidatoryFactory = new RuleValidatorFactory(new List<IRuleValidator>() { _ruleValidator1, _ruleValidator2 });
            _limitChecker = new RequestLimitChecker(_queryRepo, _ruleValidatoryFactory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _queryRepo = null;
            _ruleValidator1 = null;
            _ruleValidator2 = null;
            _ruleValidatoryFactory = null;
            _limitChecker = null;
        }

        [TestMethod]
        public void CanProcessRequest_Rule1_Test()
        {
            RequestData requestData = new RequestData();
            ClientRequest request = new ClientRequest() { ClientId = 1, ResourceId = 1 };
            requestData.ClientRequest = request;

            // Here first 5 requests in the same minute will be served and then any other requests in the same minute will be rejected. So, looking for appropraite minute
            DateTime currentTime = DateTime.Now;
            int seconds = currentTime.Second;
            if (seconds > 50)
                Thread.Sleep(10000);

            Assert.IsTrue(_limitChecker.CanProcessRequest(requestData));    // Count = 1
            Assert.IsTrue(_limitChecker.CanProcessRequest(requestData));    // Count = 2
            Assert.IsTrue(_limitChecker.CanProcessRequest(requestData));    // Count = 3
            Assert.IsTrue(_limitChecker.CanProcessRequest(requestData));    // Count = 4
            Assert.IsTrue(_limitChecker.CanProcessRequest(requestData));    // Count = 5
            Assert.IsFalse(_limitChecker.CanProcessRequest(requestData));   // Rejected
        }

        [TestMethod]
        public void CanProcessRequest_Rule2_Test()
        {
            RequestData requestData = new RequestData();
            ClientRequest request = new ClientRequest() { ClientId = 2, ResourceId = 2 };
            requestData.ClientRequest = request;

            Assert.IsTrue(_limitChecker.CanProcessRequest(requestData));
            Thread.Sleep(10000);    // Sleeping for 10 sec
            Assert.IsTrue(_limitChecker.CanProcessRequest(requestData));
            Thread.Sleep(5000);    // Sleeping for 5 sec
            Assert.IsFalse(_limitChecker.CanProcessRequest(requestData));
        }
    }
}
