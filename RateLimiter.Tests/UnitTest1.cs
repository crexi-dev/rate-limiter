using RateLimiter.Interface;
using RateLimiter.Model;
using RateLimiter.Model.Rules;
using RateLimiter.Service;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System;


namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        static IRequestService reqsvc = new RequestService();
        RateLimitingEngine eng = new RateLimitingEngine(reqsvc); //use dependency injection in prod project

        [TestInitialize]
        public void setupEngine()
        {
            // Explanation of rules
            //rule1:  acceess "aaa" max 3 times in 5 seconds
            //        AND
            //rule2:  access no more than 10 files per second
            //        AND
            //        (
            //           ( 
            //rule3:        requestor is USBasedRequestRule
            //              AND
            //rule4:        Access "bbb" max 5 times in 2 seconds
            //           )
            //           OR
            //           (
            //rule5:        requestor is not USBasedRequestRule
            //              AND
            //rule6:        Access "bbb" can only access after 2 second wait 
            //           )
            //        )

            eng.StartingRuleList.Rulelist.Add(new MaxPerTimeSpanRule() { maxcount = 3, Resource = "aaa", timespan = 5 });
            eng.StartingRuleList.Rulelist.Add(new MaxPerTimeSpanRule() { maxcount = 10, timespan = 1 });
            var secondLevelRuleList = new RuleList() { ListOperand = "Or" };
            var ThirdLevelRuleList1 = new RuleList() { ListOperand = "And" };
            var ThirdLevelRuleList2 = new RuleList() {  ListOperand = "And" };
            secondLevelRuleList.Rulelist.Add(ThirdLevelRuleList1);
            secondLevelRuleList.Rulelist.Add(ThirdLevelRuleList2);

            ThirdLevelRuleList1.Rulelist.Add(new USBasedRequestRule() { Resource = "bbb", TargetResult =true });
            ThirdLevelRuleList1.Rulelist.Add(new MaxPerTimeSpanRule() { Resource = "bbb", maxcount=5, timespan=2 });

            ThirdLevelRuleList2.Rulelist.Add(new USBasedRequestRule() { Resource = "bbb", TargetResult = false });
            ThirdLevelRuleList2.Rulelist.Add(new MinTimeSpanLapsRule() { Resource = "bbb",  Laps=2 });

            //eng.StartingRuleList.Rulelist.Add(secondLevelRuleList);
            eng.StartingRuleList.Rulelist.Add(secondLevelRuleList);
            

        }


        [TestMethod]
        public async Task Rule1Met ()
        {
            var dt = DateTime.Now;
            bool res = true;
            for (int i=0; i<3; i++)
            {
                dt = dt.AddMilliseconds(10);
                var Request = new ResourceRequest { DateTime = dt, Resource = "aaa" };
                res = res && await eng.Evaluate(Request, "US32fdgfdgbfdklfds");
            }
            Assert.IsTrue(res);
        }
        [TestMethod]
        public async Task Rule1NotMet()
        {
            var dt = DateTime.Now;
            bool res = true;
            for (int i = 0; i < 10; i++)
            {
                dt = dt.AddMilliseconds(10);
                var Request = new ResourceRequest { DateTime = dt, Resource = "aaa" };
                res = res && await eng.Evaluate(Request, "US32fdgfdgbfdklfds");
            }
            Assert.IsFalse(res);
        }

        [TestMethod]
        public async Task Rule2NotMet()
        {
            var dt = DateTime.Now;
            string resourceString = "aaa";
            bool res = true;
            for (int i = 0; i < 11; i++) 
            {
                dt = dt.AddMilliseconds(1);
                resourceString = resourceString + "a";
                var Request = new ResourceRequest { DateTime = dt, Resource = resourceString };
                res = res && await eng.Evaluate(Request, "US32fdgfdgbfdklfds");
            }
            Assert.IsFalse(res);
        }



        [TestMethod]
        public async Task TestHasRules()
        {
            var Request = new ResourceRequest { DateTime = DateTime.Now, Resource = "aaa" };
            var client = new ReqClient("US1cliwndjaslh") { Region = "EU", Subscription = "1" };
            Assert.IsTrue(await eng.Evaluate(Request,client));
            

        }
        [TestMethod]
        public async Task Rule5And6Met()
        {
            var Request = new ResourceRequest { DateTime = DateTime.Now, Resource = "bbb" };
            await eng.Evaluate(Request, "EU2gbfdklfds");
            var NewRequest = new ResourceRequest { DateTime = Request.DateTime.AddHours(1), Resource = "bbb" };
            var res = await eng.Evaluate(NewRequest, "EU2gbfdklfds");
            Assert.IsTrue(res);
        }
        [TestMethod]
        public async Task Rule5MetAnd6NotMet()
        {
            var Request = new ResourceRequest { DateTime = DateTime.Now, Resource = "bbb" };
            await eng.Evaluate(Request, "EU2gbfdklfds");
            var NewRequest = new ResourceRequest { DateTime = Request.DateTime.AddSeconds(1), Resource = "bbb" };
            var res = await eng.Evaluate(NewRequest, "EU2gbfdklfds");
            Assert.IsFalse(res); 
        }

        [TestMethod]
        public async Task Rule3And4Met()
        {
            var dt = DateTime.Now;
            bool res = true;
            for (int i = 0; i < 3; i++)
            {
                dt= dt.AddMilliseconds(100);
                var Request = new ResourceRequest { DateTime = dt, Resource = "bbb" };
                res= res && await eng.Evaluate(Request, "US2gbfdklfds");
            }

            Assert.IsTrue(res);
        }
        [TestMethod]
        public async Task Rule3And4NotMet()
        {
            var dt = DateTime.Now;
            bool res = true;
            for (int i = 0; i < 10; i++)
            {
                dt = dt.AddMilliseconds(100);
                var Request = new ResourceRequest { DateTime = dt, Resource = "bbb" };
                res = res && await eng.Evaluate(Request, "US2gbfdklfds");
            }

            Assert.IsFalse(res);
        }



    }
}