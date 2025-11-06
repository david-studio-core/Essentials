namespace DavidStudio.Core.Results;

public record FailedOperationResult : OperationResult
{
    public FailedOperationResult(params OperationResultMessage[] messages)
        : base(messages) { }

    public override bool Succeeded => false;
}