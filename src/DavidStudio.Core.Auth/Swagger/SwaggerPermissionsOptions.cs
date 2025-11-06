namespace DavidStudio.Core.Auth.Swagger;

public class SwaggerPermissionsOptions : IConfigureOptions<SwaggerGenOptions>
{
    /// <summary>
    /// Configure each API discovered for Swagger Documentation
    /// </summary>
    /// <param name="options"></param>
    public void Configure(SwaggerGenOptions options)
    {
        options.OperationFilter<OperationPermissionsFilter>();
    }
}