using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiterTests
{

    [TestClass]
    public class ApiEndpointTests
    {
        //These are integration tests to see that Ratelimiter Midlleware works as intended
        
        //To Run these you need to first run TestApi project without debugging and then run these tests.

        //NOTE: requests have different tokens not to interfere with each other.
        //For Rate Limiter this simulates effect of sending  requests from "Different Users". 

        protected static string URL = "https://localhost:44372/api/"; //TestApi URL should go here 
        protected static HttpClient Client = new HttpClient();


        //helper method for making api calls 
        private async  Task<string> GetApiResultStatus(string url, string token)
        {
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var result = await Client.GetAsync(url);
            string status = result.StatusCode.ToString();
            return status;
        }


        [TestMethod]
        public async Task Five_Second_Interval__Rule_Test_Block()
        {
            // this endpoint has 5 second interval rule set from AppSettings.json
            string url = URL + "WeatherForecast/GetOne";
            string token = "Test_Token_1";
            //This Test will make three calls with 3 second intervals making sure that limiter blocks 2 requests
            var res1 = await  GetApiResultStatus(url, token);
            Assert.IsTrue(res1 == "OK");

            Thread.Sleep(3000);
            var res2 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res2 == "TooManyRequests");

            Thread.Sleep(3000);
            var res3 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res3 == "TooManyRequests");

        }

        [TestMethod]
        public async Task Five_Second_Interval__Rule_Test_Pass()
        {
            // this endpoint has 5 second interval rule set from AppSettings.json
            string url = URL + "WeatherForecast/GetOne";
            string token = "Test_Token_2";
            //This Test will make three calls with 5 second intervals making sure that limiter does not block any requests
            var res1 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res1 == "OK");

            Thread.Sleep(5000);
            var res2 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res2 == "OK");

            Thread.Sleep(5000);
            var res3 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res3 == "OK");

        }


        [TestMethod]
        public async Task Default_Rule_Test_Block () 
        {
            //this endpoint has no endpoint rules set, so RateLimiter will run default rule for this one
            //Currently Default rule is set to 5 second interval  rule so RateLimiter should block these requests
            string url = URL + "WeatherForecast/TestEndpoint";
            string token = "Test_Token_3";

            var res1 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res1 == "OK");

            Thread.Sleep(1000);
            var res2 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res2 == "TooManyRequests");

            Thread.Sleep(1000);
            var res3 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res3 == "TooManyRequests");
        }


        [TestMethod]
        public async Task Default_Rule_Test_Pass()
        {
            //this endpoint has no endpoint rules set, so RateLimiter will run default rule for this one
            //Currently Default rule is set to 5 second interval  rule so RateLimiter should pass these requests
            string url = URL + "WeatherForecast/TestEndpoint";
            string token = "Test_Token_4";

            var res1 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res1 == "OK");

            Thread.Sleep(5000);
            var res2 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res2 == "OK");

            Thread.Sleep(5000);
            var res3 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res3 == "OK");
        }



        [TestMethod]
        public async Task TestController_Rule_Test_Block()
        {
            //this endpoint has no endpoint rules set, however there is rule set to Test Controller itself
            //Rule limits 2 requests per 10 seconds  so Rate Limiter should block 2 requests out of these 4
            string url = URL + "Test/TestEndpoint2";
            string token = "Test_Token_5";

            var res1 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res1 == "OK");

            Thread.Sleep(500);
            var res2 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res2 == "OK");

            Thread.Sleep(500);
            var res3 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res3 == "TooManyRequests");

            Thread.Sleep(500);
            var res4 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res4 == "TooManyRequests");
        }


        [TestMethod]
        public async Task TestController_Rule_Test_Pass()
        {
            //this endpoint has no endpoint rules set, however there is rule set to Test Controller itself
            //Rule limits 2 requests per 10 seconds  so Rate Limiter should block pass these requests
            string url = URL + "Test/TestEndpoint2";
            string token = "Test_Token_6";

            var res1 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res1 == "OK");

            var res2 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res2 == "OK");

            Thread.Sleep(5000);
            var res3 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res3 == "OK");

            Thread.Sleep(5000);
            var res4 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res4 == "OK");
        }

        [TestMethod]
        public async Task Combined_Rule_Tests_Block()
        {
            //this endpoint has two different rules set.
            //one rule limits 5 requests per 10 seconds, however second rule limits at least 1 second intervals between
            //RateLimiter will block 4 out of these 5 reqeusts even though they satisfy first rule
            string url = URL + "WeatherForecast/GetAll";
            string token = "Test_Token_7";

            var res1 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res1 == "OK");

            var res2 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res2 == "TooManyRequests");

            var res3 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res3 == "TooManyRequests");

            var res4 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res4 == "TooManyRequests");

            var res5 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res5 == "TooManyRequests");
        }



        [TestMethod]
        public async Task Combined_Rule_Tests_Pass()
        {
            //this endpoint has two different rules set.
            //one rule limits 5 requests per 10 seconds, however second rule limits at least 2 second intervals between
            //RateLimiter will Pass these requests as both rules are satisfied 
            string url = URL + "WeatherForecast/GetAll";
            string token = "Test_Token_8";

            var res1 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res1 == "OK");

            Thread.Sleep(1000);
            var res2 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res2 == "OK");

            Thread.Sleep(1000);
            var res3 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res3 == "OK");

            Thread.Sleep(1000);
            var res4 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res4 == "OK");

            Thread.Sleep(1000);
            var res5 = await GetApiResultStatus(url, token);
            Assert.IsTrue(res5 == "OK");
        }
    }
}
