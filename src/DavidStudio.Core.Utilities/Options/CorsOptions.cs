namespace DavidStudio.Core.Utilities.Options;

public class ApplicationCorsOptions
{
    public string? AllowedOrigins { get; init; }
    public string? AllowedMethods { get; init; }
    public string? AllowedHeaders { get; init; }
}
