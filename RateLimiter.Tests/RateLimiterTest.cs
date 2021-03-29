using Moq;
using NUnit.Framework;
using RateLimiter.Application.Interfaces;
using RateLimiter.Domain.Aggregate;
using RateLimiter.Domain.Contexts;
using RateLimiter.Domain.ValueObjects;
using RateLimiter.Infastructure.Filters;
using RateLimiter.Infastructure.Persistance;
using RateLimiter.Infastructure.Providers;
using RateLimiter.Infastructure.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private readonly Mock<IIdentityService> _identityService;
        private readonly Mock<IResourceRepository> _resourceRepository;
        private readonly IDateTime _dateTimeService;
        private readonly ILimitProvider _limitProvider;
        private readonly ILimitRuleFilter _limitRuleFilter;

        public RateLimiterTest()
        {
            _identityService = new Mock<IIdentityService>();
            _resourceRepository = new Mock<IResourceRepository>();
            _dateTimeService = new DateTimeService();
            _limitProvider = new SlidingWindowWithInterval();
            _limitRuleFilter = new GeoLocationRuleFilter();
        }

        [Test]
        public void LimitRuleFilter()
        {
            _resourceRepository.Setup(x => x.Get("order"))
               .Returns(new Resource
               {
                   Key = "order",
                   RateLimitRules = new List<LimitRule>
                   {
                       new LimitRule { GeoLocation = "US", Threshold = 5, TimeSpan = 10 },
                       new LimitRule { GeoLocation = "AU", Threshold = 3, TimeSpan = 30 },
                       new LimitRule { GeoLocation = "AU", Threshold = 1, TimeSpan = 8},
                       new LimitRule { Threshold = 1, TimeSpan = 5 }
                   }
               });

            var resource = _resourceRepository.Object.Get("order");

            var context = new RequestContext { ClientContext = new ClientContext { GeoLocation = "US" } };
            var appliedLimits = _limitRuleFilter.FilterByRequest(resource, context);
            Assert.IsTrue(appliedLimits.Count() == 1 && appliedLimits.Any(r => r.GeoLocation == "US"));

            context = new RequestContext { ClientContext = new ClientContext { GeoLocation = "AU" } };
            appliedLimits = _limitRuleFilter.FilterByRequest(resource, context);
            Assert.IsTrue(appliedLimits.Count() == 2 && appliedLimits.All(r => r.GeoLocation == "AU"));

            context = new RequestContext { ClientContext = new ClientContext { GeoLocation = "EU" } };
            appliedLimits = _limitRuleFilter.FilterByRequest(resource, context);
            Assert.IsNull(appliedLimits.FirstOrDefault().GeoLocation);
        }

        [Test]
        public void LimitProviderReachMaxInWindow()
        {
            var limitProvider = new SlidingWindowWithInterval();

            var timeStamp = _dateTimeService.Timestamp;

            var evaluationContext = new EvaluationContext
            {
                RequestContext = new RequestContext
                {
                    ClientContext = new ClientContext { Token = "test" },
                    TimeStamp = _dateTimeService.Timestamp
                },
                RuleSet = new List<LimitRule>
                {
                    new LimitRule{ Threshold = 5, TimeSpan = 10 }
                }
            };

            var visitContext = new VisitContext
            {
                Counter = 4,
                LastAccess = timeStamp - 1,
                WindowStart = timeStamp - 5
            };

            visitContext = limitProvider.Evaluate(evaluationContext, visitContext);

            // Counter Increase
            Assert.AreEqual(5, visitContext.Counter);

            // Counter Stop
            Assert.AreEqual(5, limitProvider.Evaluate(evaluationContext, visitContext).Counter);
        }

        [Test]
        public void LimitProviderUpdatedWindow()
        {
            var limitProvider = new SlidingWindowWithInterval();

            var timeStamp = _dateTimeService.Timestamp;

            var evaluationContext = new EvaluationContext
            {
                RequestContext = new RequestContext
                {
                    ClientContext = new ClientContext { Token = "test" },
                    TimeStamp = _dateTimeService.Timestamp
                },
                RuleSet = new List<LimitRule>
                {
                    new LimitRule{ Threshold = 5, TimeSpan = 10 } // Represents 5 request per 10 second
                }
            };

            var visitContext = new VisitContext
            {
                Counter = 4,
                LastAccess = timeStamp - 1,
                WindowStart = timeStamp - 11
            };

            visitContext = limitProvider.Evaluate(evaluationContext, visitContext);

            Assert.IsTrue(visitContext.Counter == 1);
            Assert.IsTrue(visitContext.LastAccess == timeStamp);
            Assert.IsTrue(visitContext.WindowStart == timeStamp);
        }

        [Test]
        public void LimitProviderIntervalReject()
        {
            var limitProvider = new SlidingWindowWithInterval();

            var timeStamp = _dateTimeService.Timestamp;

            var evaluationContext = new EvaluationContext
            {
                RequestContext = new RequestContext
                {
                    ClientContext = new ClientContext { Token = "test" },
                    TimeStamp = _dateTimeService.Timestamp
                },
                RuleSet = new List<LimitRule>
                {
                    new LimitRule{ Threshold = 1, TimeSpan = 10 } // a request per 10 seconds
                }
            };

            var visitContext = new VisitContext
            {
                Counter = 1,
                LastAccess = timeStamp - 5,
                WindowStart = timeStamp - 5
            };

            Assert.IsTrue(visitContext.Equals(limitProvider.Evaluate(evaluationContext, visitContext)));

            evaluationContext.RequestContext.TimeStamp += 6;
            visitContext = limitProvider.Evaluate(evaluationContext, visitContext);

            Assert.IsTrue(visitContext.LastAccess == evaluationContext.RequestContext.TimeStamp);
        }

        [Test]
        public void ApiConcurrentRequestsIntervalLimit()
        {
            var tokens = new Dictionary<string, string>();
            tokens["US"] = "token1";

            _identityService.Setup(x => x.VerifyToken(tokens["US"])).Returns(new ClientContext { GeoLocation = "US", Token = tokens["US"] });

            _resourceRepository.Setup(x => x.Get("order"))
                .Returns(new Resource
                {
                    Key = "order",
                    RateLimitRules = new List<LimitRule>
                    {
                        new LimitRule { GeoLocation = "US", Threshold = 1, TimeSpan = 2 },
                    }
                });

            var tokenBucket = new InMemoryTokenBucket(_limitProvider);
            var limitService = new RequestLimitService(tokenBucket, _resourceRepository.Object, _limitRuleFilter);
            var api = new Api(_identityService.Object, _dateTimeService, limitService);

            var accepted = 0;
            var rejected = 0;
            Parallel.For(0, 20, (i) =>
            {
                var result = api.ReceiveRequest("order", tokens["US"]);
                if(result == Api.Accepted)
                {
                    Interlocked.Increment(ref accepted);
                }
                else
                {
                    Interlocked.Increment(ref rejected);
                }
            });

            Assert.AreEqual(1, accepted);
            Assert.AreEqual(19, rejected);

            Thread.Sleep(2000);
            Assert.AreEqual(Api.Accepted,api.ReceiveRequest("order", tokens["US"]));
        }

        [Test]
        public void ApiConcurrentRequestsSlidingWindowLimit()
        {
            var tokens = new Dictionary<string, string>();
            tokens["US"] = "token1";

            _identityService.Setup(x => x.VerifyToken(tokens["US"])).Returns(new ClientContext { GeoLocation = "US", Token = tokens["US"] });

            _resourceRepository.Setup(x => x.Get("order"))
                .Returns(new Resource
                {
                    Key = "order",
                    RateLimitRules = new List<LimitRule>
                    {
                        new LimitRule { GeoLocation = "US", Threshold = 10, TimeSpan = 500 },
                    }
                });

            var tokenBucket = new InMemoryTokenBucket(_limitProvider);
            var limitService = new RequestLimitService(tokenBucket, _resourceRepository.Object, _limitRuleFilter);
            var api = new Api(_identityService.Object, _dateTimeService, limitService);

            var accepted = 0;
            var rejected = 0;
            Parallel.For(0, 20, (i) =>
            {
                var result = api.ReceiveRequest("order", tokens["US"]);
                if (result == Api.Accepted)
                {
                    Interlocked.Increment(ref accepted);
                }
                else
                {
                    Interlocked.Increment(ref rejected);
                }
            });

            Assert.AreEqual(10, accepted);
            Assert.AreEqual(10, rejected);
        }

        [Test]
        public void ApiMultiGeoLocationAccess()
        {
            var tokens = new Dictionary<string, string>();
            tokens["US"] = "token1";
            tokens["EU"] = "token2";
            tokens["AU"] = "token3";

            _identityService.Setup(x => x.VerifyToken(tokens["US"])).Returns(new ClientContext { GeoLocation = "US", Token = tokens["US"] });
            _identityService.Setup(x => x.VerifyToken(tokens["EU"])).Returns(new ClientContext { GeoLocation = "EU", Token = tokens["EU"] });
            _identityService.Setup(x => x.VerifyToken(tokens["AU"])).Returns(new ClientContext { GeoLocation = "AU", Token = tokens["AU"] });

            _resourceRepository.Setup(x => x.Get("order"))
                .Returns(new Resource
                {
                    Key = "order",
                    RateLimitRules = new List<LimitRule>
                    {
                        new LimitRule { GeoLocation = "US", Threshold = 5, TimeSpan = 10 },
                        new LimitRule { GeoLocation = "AU", Threshold = 1, TimeSpan = 3 },
                        new LimitRule { GeoLocation = "AU", Threshold = 3, TimeSpan = 10 },
                        new LimitRule { Threshold = 1, TimeSpan = 10 },
                    }
                });
            _resourceRepository.Setup(x => x.Get("product"))
                .Returns(new Resource
                {
                    Key = "product"
                });

            var tokenBucket = new InMemoryTokenBucket(_limitProvider);
            var limitService = new RequestLimitService(tokenBucket, _resourceRepository.Object, _limitRuleFilter);
            var api = new Api(_identityService.Object, _dateTimeService, limitService);

            api.ReceiveRequest("order", tokens["US"]);
            api.ReceiveRequest("order", tokens["AU"]);
            api.ReceiveRequest("order", tokens["EU"]);
            api.ReceiveRequest("product", tokens["AU"]);

            Thread.Sleep(2000);
            Assert.AreEqual(Api.Accepted, api.ReceiveRequest("order", tokens["US"]));
            Assert.AreEqual(Api.Rejected, api.ReceiveRequest("order", tokens["AU"]));
            Assert.AreEqual(Api.Rejected, api.ReceiveRequest("order", tokens["EU"]));

            Thread.Sleep(2000);
            Assert.AreEqual(Api.Accepted, api.ReceiveRequest("order", tokens["US"]));
            Assert.AreEqual(Api.Accepted, api.ReceiveRequest("order", tokens["AU"]));
            Assert.AreEqual(Api.Rejected, api.ReceiveRequest("order", tokens["EU"]));
            Assert.AreEqual(Api.Accepted, api.ReceiveRequest("product", tokens["AU"]));

            Thread.Sleep(2000);
            Assert.AreEqual(Api.Accepted, api.ReceiveRequest("order", tokens["US"]));
            Assert.AreEqual(Api.Rejected, api.ReceiveRequest("order", tokens["AU"]));
            Assert.AreEqual(Api.Rejected, api.ReceiveRequest("order", tokens["EU"]));
            Assert.AreEqual(Api.Accepted, api.ReceiveRequest("product", tokens["AU"]));

            Thread.Sleep(5000);
            Assert.AreEqual(Api.Accepted, api.ReceiveRequest("order", tokens["US"]));
            Assert.AreEqual(Api.Accepted, api.ReceiveRequest("order", tokens["AU"]));
            Assert.AreEqual(Api.Accepted, api.ReceiveRequest("order", tokens["EU"]));
            Assert.AreEqual(Api.Accepted, api.ReceiveRequest("product", tokens["AU"]));
        }
    }
}
