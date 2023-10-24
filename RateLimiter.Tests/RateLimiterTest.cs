using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using RateLimiter.Contexts;
using RateLimiter.Middlewares;
using RateLimiter.Models;
using RateLimiter.RateLimitHandlers;
using RateLimiter.Services;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{
    private HttpContext GetHttpContext(string token)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        httpContext.Request.QueryString = httpContext.Request.QueryString.Add("culture", "de-DE");
        
        return httpContext;
    }

    //private async Task ProcessMiddleware(IOptions<RateLimitOptions> options)
    //{ 
    //    var ProcessFactory = DependencyHelper.GetRequiredService<ProcessFactory>(false);
        //var httpContext = GetHttpContext();
        //var reqDelegate = new RequestDelegate(httpContext);
        ////Func<HttpContext, Task> reqDelegate = innerHttpContext =>
        ////{
        ////    innerHttpContext.Response.WriteAsync("");
        ////    return Task.CompletedTask;
        ////};
        //var middleware = new RateLimitMiddleware(next: reqDelegate, options, ProcessFactory);
        //await middleware.InvokeAsync(httpContext, reqDelegate);

    //}

    [Test(Description = "Client 1 Beginning of the border - call one after the other")]
    public async Task Example()
	{
        var cach = DependencyHelper.GetRequiredService<ICacheService>(true);
        var options = DependencyHelper.GetRequiredService<IOptions<RateLimitOptions>>(false);
        var processFactory  = DependencyHelper.GetRequiredService<ProcessFactoryBase>(false);
        var client = new ClientRequestIdentity() { ClientId = "client_1_somekey" };

       // Assert.IsNull(options.Value, "options.Value Is Null");
        //var client1Opts = options.Value.ClientOptions.FirstOrDefault(x=>x.ClientId == client.ClientId);

        TestContext.WriteLine("Storing one call record");
        await cach.StoreData(new RequestHistoryEModel() { Id = Guid.NewGuid(), ClientId = "client_1_somekey", ReqDate = DateTime.UtcNow });
        var res = await processFactory.Check(client);
        TestContext.WriteLine(res.Message);
        //await middleware.InvokeAsync(GetHttpContext(), );
        Assert.That(true, Is.True);
	}
}