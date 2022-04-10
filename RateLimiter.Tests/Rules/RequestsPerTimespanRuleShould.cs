using RateLimiter.Common;
using RateLimiter.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;
using Shouldly;
using Microsoft.Extensions.Options;

namespace RateLimiter.Tests.Rules
{
    public class RequestsPerTimespanRuleShould
    {
        private readonly IApiClient apiClient;
        private readonly IRateLimiterRepository repository;
        private readonly IOptions<RequestPerTimeSpanOptions> options;

        private RequestsPerTimespanRule rule;

        public RequestsPerTimespanRuleShould()
        {
            apiClient = Substitute.For<IApiClient>();
            apiClient.ClientId.Returns(new Guid());

            repository = Substitute.For<IRateLimiterRepository>();
            options = Substitute.For<IOptions<RequestPerTimeSpanOptions>>();
            options.Value.Returns(new RequestPerTimeSpanOptions
            {
                MaxAlloweRequests = 10,
                WithinTimeSpan = new TimeSpan(0, 1, 0, 0)
            });

            rule = new RequestsPerTimespanRule(apiClient, repository, options);
        }

        [Fact]
        public void Return_True_If_Requests_Are_Less_Than_Max_Allowed()
        {
            repository.GetAmountOfLoginsSinceTimespan(Arg.Any<Guid>(), Arg.Any<TimeSpan>())
                .Returns(8);

            var result = rule.Validate();
            result.ShouldBeTrue();
        }

        [Fact]
        public void Return_False_If_Requests_Are_Less_Than_Max_Allowed()
        {
            repository.GetAmountOfLoginsSinceTimespan(Arg.Any<Guid>(), Arg.Any<TimeSpan>())
                .Returns(12);

            var result = rule.Validate();
            result.ShouldBeFalse();
        }
    }
}
