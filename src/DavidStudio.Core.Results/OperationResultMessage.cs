using System.Text.Json.Serialization;

namespace DavidStudio.Core.Results;

public record OperationResultMessage
{
    public OperationResultMessage(string message, OperationResultSeverity severity)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Severity = severity;
    }

    public string Message { get; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OperationResultSeverity Severity { get; }
}