using DavidStudio.Core.Swagger.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DavidStudio.Core.Swagger.Configurations;

/// <summary>
/// Configures Swagger to support OAuth2 authentication in the Swagger UI.
/// </summary>
/// <remarks>
/// This class implements <see cref="IConfigureOptions{SwaggerGenOptions}"/> and sets up:
/// <list type="bullet">
/// <item>A security definition named "OAuth2".</item>
/// <item>A security requirement so that endpoints can be tested with the Bearer token in Swagger UI.</item>
/// </list>
/// </remarks>
public class OAuth2AuthenticationSwaggerOptions(IOptions<OAuth2CodeFlowOptions> oauth) : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        var scopes = oauth.Value.Scopes.ToDictionary(s => s, s => $"Access {s}");

        options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
        {
            Name = "OAuth2 Authorization",
            Type = SecuritySchemeType.OAuth2,
            Scheme = "Bearer",
            In = ParameterLocation.Header,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(oauth.Value.AuthorizationUrl),
                    TokenUrl = new Uri(oauth.Value.TokenUrl),
                    Scopes = scopes
                }
            }
        });

        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("OAuth2", document)] = []
        });
    }
}