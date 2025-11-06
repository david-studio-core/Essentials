namespace DavidStudio.Core.Results;

public record SuccessfulOperationResult : OperationResult
{
    public SuccessfulOperationResult(params OperationResultMessage[] messages)
        : base(messages) { }

    public override bool Succeeded => true;
}