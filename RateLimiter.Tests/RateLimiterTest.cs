using NUnit.Framework;
using RateLimiter.Model.Resource;
using RateLimiter.Model.Response;
using System.Threading;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{
	[Test]
	public void TestResourceGetListings()
	{
		var request = new ResourceRequest
		{
			IpAddress = HelperRateLimiter.IP_WHITE_LIST,
			Resource = EnumResource.GetListings,
			Token = HelperRateLimiter.TOKEN
		};

		var service = HelperRateLimiter.GetServiceRateLimiter();
		var response = service.Validate(request);

		Assert.That(response.Success, Is.True);
	}

    [Test]
    public void TestResourceGetListingsBlackList()
    {
        var request = new ResourceRequest
        {
            IpAddress = HelperRateLimiter.IP_BLACK_LIST,
            Resource = EnumResource.GetListings,
            Token = HelperRateLimiter.TOKEN
        };

        var service = HelperRateLimiter.GetServiceRateLimiter();
        var response = service.Validate(request);

        Assert.That(response.Success, Is.False);
        HelperRateLimiter.OutputResponse(service.GetRequestLogAll());
    }

    [Test]
    public void TestResourceGetListingsTimespan()
    {
        var request = new ResourceRequest
        {
            IpAddress = HelperRateLimiter.IP_WHITE_LIST,
            Resource = EnumResource.GetListings,
            Token = HelperRateLimiter.TOKEN
        };

        var service = HelperRateLimiter.GetServiceRateLimiter();

        ResponseRateLimitValidation response = null;

        for (int i = 0; i < HelperRateLimiter.DEFAULT_REQUEST_THRESHOLD; i++)
            response = service.Validate(request);

        Assert.That(response.Success, Is.True);
        HelperRateLimiter.OutputResponse(service.GetRequestLogAll());
    }

    [Test]
    public void TestResourceGetListingsTimespanExceeded()
    {
        var request = new ResourceRequest
        {
            IpAddress = HelperRateLimiter.IP_WHITE_LIST,
            Resource = EnumResource.GetListings,
            Token = HelperRateLimiter.TOKEN
        };

        var service = HelperRateLimiter.GetServiceRateLimiter();

        ResponseRateLimitValidation response = null;

        for (int i = 0; i < HelperRateLimiter.DEFAULT_REQUEST_THRESHOLD + 5; i++)
            response = service.Validate(request);

        Assert.That(response.Success, Is.False);
        HelperRateLimiter.OutputResponse(service.GetRequestLogAll());
    }

    [Test]
    public void TestResourceGetListingsTimespanEU()
    {
        var request = new ResourceRequest
        {
            IpAddress = HelperRateLimiter.IP_WHITE_LIST,
            Resource = EnumResource.GetListings,
            Token = HelperRateLimiter.EU_TOKEN
        };

        var service = HelperRateLimiter.GetServiceRateLimiter();
        var delay = (HelperRateLimiter.DEFAULT_TIMESPAN_SECONDS + 2) * 1000; 

        ResponseRateLimitValidation response = null;

        for (int i = 0; i < HelperRateLimiter.DEFAULT_REQUEST_THRESHOLD; i++)
        {
            Thread.Sleep(delay);
            response = service.Validate(request);
            Assert.That(response.Success, Is.True);
        }

        HelperRateLimiter.OutputResponse(service.GetRequestLogAll());
    }

    [Test]
    public void TestResourceGetListingsTimespanExceededEU()
    {
        var request = new ResourceRequest
        {
            IpAddress = HelperRateLimiter.IP_WHITE_LIST,
            Resource = EnumResource.GetListings,
            Token = HelperRateLimiter.EU_TOKEN
        };

        var service = HelperRateLimiter.GetServiceRateLimiter();                

        var response = service.Validate(request);
        Assert.That(response.Success, Is.True);

        //back to back so we should brick here
        response = service.Validate(request);
        Assert.That(response.Success, Is.False);

        HelperRateLimiter.OutputResponse(service.GetRequestLogAll());
    }

    [Test]
    public void TestResourceGetListingDetail()
    {
        var request = new ResourceRequest
        {
            IpAddress = HelperRateLimiter.IP_WHITE_LIST,
            Resource = EnumResource.GetListingDetail,
            Token = HelperRateLimiter.TOKEN
        };

        var service = HelperRateLimiter.GetServiceRateLimiter();
        var response = service.Validate(request);

        Assert.That(response.Success, Is.True);

        //swap to a non whitelisted ip
        request.IpAddress = "123.55.123";

        response = service.Validate(request);

        Assert.That(response.Success, Is.False);

        HelperRateLimiter.OutputResponse(service.GetRequestLogAll());
    }

    [Test]
    public void TestResourceCreateListingInquiry()
    {
        var request = new ResourceRequest
        {
            IpAddress = HelperRateLimiter.IP_WHITE_LIST,
            Resource = EnumResource.CreateListingInquiry,
            Token = HelperRateLimiter.TOKEN
        };

        var service = HelperRateLimiter.GetServiceRateLimiter();
        var response = service.Validate(request);

        //disabled
        Assert.That(response.Success, Is.False);

        HelperRateLimiter.OutputResponse(service.GetRequestLogAll());
    }

    [Test]
    public void TestResourceCreateListingFavorite()
    {
        var request = new ResourceRequest
        {
            IpAddress = HelperRateLimiter.IP_WHITE_LIST,
            Resource = EnumResource.CreateListingFavorite,
            Token = HelperRateLimiter.TOKEN
        };

        var service = HelperRateLimiter.GetServiceRateLimiter();
        var response = service.Validate(request);

        //no configuration
        Assert.That(response.Success, Is.False);

        HelperRateLimiter.OutputResponse(service.GetRequestLogAll());
    }
}