using Microsoft.OpenApi.Models;
using RateLimiter;
using RateLimiter.ServiceConfiguratoin;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


builder.Services.AddControllers();

builder.Services.RegisterRateLimiterServices(builder.Configuration);

var app = builder.Build();


app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<RateLimiterMiddleware>();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
