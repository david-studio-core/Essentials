namespace DavidStudio.Core.Swagger.Options;

public class OAuth2CodeFlowOptions
{
    public string AuthorizationUrl { get; set; } = string.Empty;
    public string TokenUrl { get; set; } = string.Empty;
    public string[] Scopes { get; set; } = [];
}