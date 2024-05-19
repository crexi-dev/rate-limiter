using RateLimiter.Repositories.Detectors;
using RateLimiter.Repositories;
using RateLimiter.RuleTemplates;
using Unity;
using RateLimiter.Rules.RequestConverters;

namespace RateLimiter.Tests.Helpers
{
    public static class ContainerHelper
    {
        public static IUnityContainer RegisterDependencies(this IUnityContainer container)
        {
            container.RegisterType<IRuleTemplateDetector, DefaultRuleTemplateDetector>()
                .RegisterSingleton<IRuleRepository, RuleRepository>()
                .RegisterType<IRuleFactory, RuleFactory>()
                .RegisterType<IRuleConstructorDetector, RuleConstructorDetector>()
                .RegisterType<IRequestConverterFactory, RequestConverterFactory>()
                .RegisterSingleton<IRequestLogRepository, RequestLogRepository>()
                .RegisterType<IRequestConverterDetector, RequestConverterDetector>();
            return container;
        }
    }
}
