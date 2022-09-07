using Core.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RateLimiter;
using System.Collections.Generic;

namespace WeatherForecastApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public delegate IRateLimitHandler ServiceResolver(RateLimitStrategyEnum key);

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMemoryCache();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WeatherForecastApi", Version = "v1" });
            });

            services.AddTransient<CertainTimespanPassedSinceLastCallHandler>();
            services.AddTransient<LimitedRequestPerTimespanHandler>();

            services.AddTransient<ServiceResolver>(serviceProvider => key =>
            {
                switch (key)
                {
                    case RateLimitStrategyEnum.CertainTimespanPassedSinceLastCall:
                        return serviceProvider.GetService<CertainTimespanPassedSinceLastCallHandler>();
                    case RateLimitStrategyEnum.LimitedRequestPerTimespan:
                        return serviceProvider.GetService<LimitedRequestPerTimespanHandler>();
                    default:
                        throw new KeyNotFoundException();
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherForecastApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseMiddleware<RateLimitMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
