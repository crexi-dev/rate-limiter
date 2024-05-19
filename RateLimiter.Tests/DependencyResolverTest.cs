using RateLimiter.Repositories;
using RateLimiter.Repositories.Detectors;
using RateLimiter.RuleTemplates;
using Unity;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class DependencyResolverTest
    {
        private IUnityContainer _unity = new UnityContainer().AddExtension(new Diagnostic());
        [OneTimeSetUp]
        public void SetUpFixture()
        {
            _unity.RegisterType<IRuleTemplateDetector, DefaultRuleTemplateDetector>();
            _unity.RegisterSingleton<IRuleRepository, RuleRepository>();
            _unity.RegisterType<IRuleFactory, RuleFactory>();
            _unity.RegisterType<IRuleConstructorDetector, RuleConstructorDetector>();
            _unity.RegisterType<IRequestConverterFactory, RequestConverterFactory>();
            _unity.RegisterType<IRequestLogRepository, RequestLogRepository>();
            _unity.RegisterType<IRequestConverterDetector, RequestConverterDetector>();
        }

        [Test]
        public void ResolveAPIClass()
        {
            Should.NotThrow(() => _unity.Resolve<RuleBuilder>());
            Should.NotThrow(() => _unity.Resolve<RequestValidator>());
        }
    }
}
