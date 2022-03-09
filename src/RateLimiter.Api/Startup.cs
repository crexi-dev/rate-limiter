using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RateLimiter.Filters;
using RateLimiter.Api.Attributes;
using RateLimiter.Services.Weather;

namespace RateLimiter.Api
{
    /// <summary>
    /// Startup class
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllers(config =>
            {
                //Add global authorization filter
                config.Filters.Add(new AuthorizationFilter());
            });

            //Add API versioning to application
            services.AddApiVersioning(
                options =>
                {
                    //Reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
                    options.ReportApiVersions = true;

                    options.AssumeDefaultVersionWhenUnspecified = true;
                });

            services.AddVersionedApiExplorer(
                options =>
                {
                    //Add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    //NOTE: the specified format code will format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";

                    //NOTE: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    //can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Throttled API",
                        Description = "A sample API demonstrating the rate limiter pattern.",
                        Contact = new OpenApiContact
                        {
                            Name = "Ben Chen",
                            Url = new Uri("https://linkedin.com/in/bchen04")
                        }
                    });

                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "Authorization header using the Bearer scheme (i.e. Bearer 12345abcdef).",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new List<string>()
                        }
                    });

                    options.DescribeAllParametersInCamelCase();

                    // Set the comments path for the Swagger JSON and UI.
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    options.IncludeXmlComments(xmlPath);
                });

            // Register the services with a scoped lifetime
            services.AddScoped<IWeatherForecastService, WeatherForecastService>();
            services.AddScoped<MaxRequestsThrottleAttribute>();
            services.AddScoped<MaxSizeThrottleAttribute>();
            services.AddScoped<SaveClientMetricsFilter>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
