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
            
        }

        public void ConfigureUnityForResolveAPIClasses()
        {

        }

        [Test]
        public void ResolveAPIClass()
        {
            Should.NotThrow(() => _unity.Resolve<RuleBuilder>());
            Should.NotThrow(() => _unity.Resolve<RequestValidator>());
        }
    }
}
