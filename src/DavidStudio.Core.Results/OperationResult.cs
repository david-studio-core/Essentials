using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace DavidStudio.Core.Results;

public abstract record OperationResult
{
    private protected OperationResult(params OperationResultMessage[] messages)
    {
        Messages = [.. messages];
    }

    public virtual bool Succeeded { get; }

    public virtual bool HasWarnings() => FindWarnings().Any();
    public virtual bool HasErrors() => FindErrors().Any();

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OperationStatus Status
    {
        get
        {
            if (Succeeded)
                return OperationStatus.Success;

            if (HasWarnings())
                return OperationStatus.PartialSuccess;

            return OperationStatus.Failure;
        }
    }

    public ImmutableList<OperationResultMessage> Messages { get; }

    public static OperationResult Success(params OperationResultMessage[] messages) =>
        new SuccessfulOperationResult(messages);

    public static OperationResult Failure(params OperationResultMessage[] errors) =>
        new FailedOperationResult(errors);

    private IEnumerable<OperationResultMessage> FindWarnings() =>
        Messages.Where(x => x.Severity == OperationResultSeverity.Warning);

    private IEnumerable<OperationResultMessage> FindErrors() =>
        Messages.Where(x => x.Severity == OperationResultSeverity.Error);
}