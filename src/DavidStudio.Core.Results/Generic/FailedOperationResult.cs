namespace DavidStudio.Core.Results.Generic;

public record FailedOperationResult<T> : OperationResult<T>
{
    public FailedOperationResult(T? value, params OperationResultMessage[] messages)
        : base(value, messages) { }

    public override bool Succeeded => false;
}