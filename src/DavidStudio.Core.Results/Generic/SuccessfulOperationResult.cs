namespace DavidStudio.Core.Results.Generic;

public record SuccessfulOperationResult<T> : OperationResult<T>
{
    public SuccessfulOperationResult(T value, params OperationResultMessage[] messages)
        : base(value, messages) { }

    public override bool Succeeded => true;
}