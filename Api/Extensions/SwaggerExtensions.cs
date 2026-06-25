using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Sanaclub.Api.Swagger;

namespace Sanaclub.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddApiSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Sanaclub API",
                Version = "v1"
            });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Paste the full value: Bearer {your JWT access token}",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            };

            options.AddSecurityDefinition("Bearer", securityScheme);

            options.OperationFilter<AuthorizeOperationFilter>();
        });

        return services;
    }
}
