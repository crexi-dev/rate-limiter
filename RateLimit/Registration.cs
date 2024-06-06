using DataBaseLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RateLimit.Contracts;
using RateLimit.Restictions;
using RateLimit.Rules;
using RateLimit.Services;

namespace RateLimit
{
	public static class Registration
	{
		public static IServiceCollection AddRateRuleAService(this IServiceCollection services, IConfiguration configuration)
		{
			AddLocalServices(services, configuration);
			services.AddSingleton<IRule, RuleA>();

			return services;
		}
		public static IServiceCollection AddRateRuleBService(this IServiceCollection services, IConfiguration configuration)
		{
			AddLocalServices(services, configuration);
			services.AddSingleton<IRule, RuleB>();
			return services;
		}
		public static IServiceCollection AddRateRuleCService(this IServiceCollection services, IConfiguration configuration)
		{
			AddLocalServices(services, configuration);
			services.AddSingleton<IRule, RuleC>();
			return services;
		}

		private static IServiceCollection AddLocalServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<Options.RuleOptions>(configuration.GetSection(Options.RuleOptions.RuleOption));
			services.AddDbContext<DataBaseContext>(options =>
				options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
			services.AddScoped<IDataAccess, RateLimit.DataAccess.DataAccess>();
			services.AddScoped<ILimitService, LimitService>();
			services.AddSingleton<IRestriction, TimeSpanPassedSinceLastCallRestictions>();
			services.AddSingleton<IRestriction, XRequestPerDatespanRestictions>();

			return services;
		}
	}
}
