using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RateLimiter.Filters;
using RateLimiter.Services;
using RateLimiter.Storage;
using RateLimiter.Web;
using RateLimiter.Web.Controllers;

namespace RateLimiter.Web.Tests.Controllers
{
    [TestClass]
    public class ValuesControllerTest
    {
        private Mock<InMemoryStorageManager> mockStorageManager { get; set; }
        private AccessService accessService { get; set; }
        private ValuesController controller { get; set; }

        [TestInitialize]
        public void BeforeTest() {
            mockStorageManager = new Mock<InMemoryStorageManager>();
            accessService = new AccessService(mockStorageManager.Object);
            controller = new ValuesController(accessService);

            InitializeApiController(controller);
        }

        [TestCleanup]
        public void CleanUpTest() {
        }

        [TestMethod]
        public async Task GetNoLimitEndpoint_Should_Return_200()
        {
            var response = await controller.GetFromNoLimitEndpoint().ExecuteAsync(CancellationToken.None);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Content.ReadAsStringAsync().Result));
        }

        [TestMethod]
        public async Task GetLimitedByTimeSpan_Should_Return_200()
        {
            var response = await controller.GetLimitedByTimeSpan().ExecuteAsync(CancellationToken.None);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Content.ReadAsStringAsync().Result));
        }

        [TestMethod]
        [ExpectedException(typeof(HttpException))]
        public async Task RateLimiterPerTimeSpan_Should_Return_403_On_Second_Call() {
            var context = new HttpActionContext();
            context.ControllerContext = controller.ControllerContext;
            context.ControllerContext.Controller = controller;
            var filter = new RateLimiterPerTimeSpanAttribute(30, true);

            filter.OnActionExecuting(context);
            //makes first call so we save the access.
            var response = await controller.GetLimitedByTimeSpan().ExecuteAsync(CancellationToken.None);
            filter.OnActionExecuting(context);
        }

        [TestMethod]
        public async Task GetLimitedByRequestRate_Should_Return_200()
        {
            var response = await controller.GetLimitedByRequestRate().ExecuteAsync(CancellationToken.None);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Content.ReadAsStringAsync().Result));
        }

        [TestMethod]
        [ExpectedException(typeof(HttpException))]
        public async Task GetLimitedByRequestRate_Should_Return_403_On_Second_Call()
        {
            var context = new HttpActionContext();
            context.ControllerContext = controller.ControllerContext;
            context.ControllerContext.Controller = controller;
            var filter = new RateLimiterRequestRateAttribute(60, 1, true);

            filter.OnActionExecuting(context);
            //makes first call so we save the access.
            var response = await controller.GetLimitedByRequestRate().ExecuteAsync(CancellationToken.None);
            filter.OnActionExecuting(context);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpException))]
        public void RateLimiterBlockIpRangeAttribute_Should_Return_403_For_Disallowed_IP()
        {
            var context = new HttpActionContext();
            context.ControllerContext = controller.ControllerContext;
            context.ControllerContext.Controller = controller;
            var filter = new RateLimiterBlockIpRangeAttribute("127.0.0.0", "127.0.0.99");

            filter.OnActionExecuting(context);
        }      

        [TestMethod]
        public async Task RateLimiterBlockIpRangeAttribute_Should_Return_200_For_Allowed_IP()
        {
            var context = new HttpActionContext();
            context.ControllerContext = controller.ControllerContext;
            context.ControllerContext.Controller = controller;
            var filter = new RateLimiterBlockIpRangeAttribute("127.0.0.20", "127.0.0.99");

            filter.OnActionExecuting(context);

            var response = await controller.GetFromBlockedEndpoint().ExecuteAsync(CancellationToken.None);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Content.ReadAsStringAsync().Result));
        }

        [TestMethod]
        [ExpectedException(typeof(HttpException))]
        public async Task GetLimitedPerTokenRegion_US_Token_Should_Return_403_On_Second_Call()
        {
            var context = new HttpActionContext();
            context.ControllerContext = controller.ControllerContext;
            context.ControllerContext.Controller = controller;
            var filter = new RateLimiterByRegionAttribute(60, 1, 30, true);

            context.ControllerContext.Request.Headers.Remove("token");
            context.ControllerContext.Request.Headers.Add("token", "us-token");

            filter.OnActionExecuting(context);
            //makes first call so we save the access.
            var response = await controller.GetLimitedByRequestRate().ExecuteAsync(CancellationToken.None);
            filter.OnActionExecuting(context);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpException))]
        public async Task GetLimitedPerTokenRegion_EU_Token_Should_Return_403_On_Second_Call()
        {
            var context = new HttpActionContext();
            context.ControllerContext = controller.ControllerContext;
            context.ControllerContext.Controller = controller;
            var filter = new RateLimiterByRegionAttribute(60, 1, 30, true);

            context.ControllerContext.Request.Headers.Remove("token");
            context.ControllerContext.Request.Headers.Add("token", "eu-token");

            filter.OnActionExecuting(context);
            //makes first call so we save the access.
            var response = await controller.GetLimitedByRequestRate().ExecuteAsync(CancellationToken.None);
            filter.OnActionExecuting(context);
        }

        private void InitializeApiController(BaseApiController baseApiController)
        {
            HttpConfiguration httpConfig = new HttpConfiguration();
            HttpRequestMessage httpRequest = new HttpRequestMessage();

            httpRequest.Headers.Add("account", "1");
            httpRequest.Headers.Add("token", "abcde");
            httpRequest.Headers.Add("ip", "127.0.0.1");

            var routeData = new HttpRouteData(new HttpRoute(""));
            
            baseApiController.ControllerContext = new HttpControllerContext(httpConfig, routeData, httpRequest)
            {
                Configuration = httpConfig
            };

            HttpContext.Current = new HttpContext(new HttpRequest(null, "http://localhost:52552", null), new HttpResponse(null));
        }
    }
}
