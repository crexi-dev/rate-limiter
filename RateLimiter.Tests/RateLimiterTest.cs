using Moq;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WebApi;
using WebApi.Controllers;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private WeatherForecastController? _controller;

        [SetUp]
        public void Setup()
        {
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var httpClient = new HttpClient();
            httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            _controller = new WeatherForecastController();
        }


        [Test]
        public void Get_ReturnsExpectedForecasts()
        {
            var result = _controller?.Get();
            Assert.IsInstanceOf<WeatherForecast[]>(result);
        }

        [Test]
        public async Task Get_CalledThreeTimesWithinTenSeconds_ReturnsTooManyRequestsOnFourthCall()
        {
            for (int i = 0; i < 3; i++)
            {
                await MakeApiCall();
            }

            var tooManyRequestsResult = await MakeApiCall();

            Assert.IsInstanceOf<HttpResponseMessage>(tooManyRequestsResult);
            Assert.AreEqual(HttpStatusCode.TooManyRequests, tooManyRequestsResult.StatusCode);
        }

        //[Test]
        //public async Task Get_CalledThreeTimesForLongTimeSpan_ReturnsOk()
        //{
        //    for (int i = 0; i < 3; i++)
        //    {
        //        await MakeApiCall();
        //        await Task.Delay(5000);
        //    }

        //    var tooManyRequestsResult = await MakeApiCall();

        //    Assert.IsInstanceOf<HttpResponseMessage>(tooManyRequestsResult);
        //    Assert.AreEqual(HttpStatusCode.OK, tooManyRequestsResult.StatusCode);
        //}

        private static async Task<HttpResponseMessage> MakeApiCall()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("accessToken", "111");

            return await httpClient.GetAsync("https://localhost:7013/weatherforecast");
        }
    }
}