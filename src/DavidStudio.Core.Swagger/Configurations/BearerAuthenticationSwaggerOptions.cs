using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DavidStudio.Core.Swagger.Configurations;

/// <summary>
/// Configures Swagger to support JWT Bearer authentication in the Swagger UI.
/// </summary>
/// <remarks>
/// This class implements <see cref="IConfigureOptions{SwaggerGenOptions}"/> and sets up:
/// <list type="bullet">
/// <item>A security definition named "Bearer" for JWT tokens.</item>
/// <item>A security requirement so that endpoints can be tested with the Bearer token in Swagger UI.</item>
/// </list>
/// </remarks>
public class BearerAuthenticationSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.",
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                ["Bearer"]
            }
        });
    }
}