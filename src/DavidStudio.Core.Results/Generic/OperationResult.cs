using System.Diagnostics.CodeAnalysis;

namespace DavidStudio.Core.Results.Generic;

public abstract record OperationResult<T> : OperationResult
{
    private protected OperationResult(T? value, params OperationResultMessage[] messages)
        : base(messages)
    {
        Value = value;
    }

    public T? Value { get; }

    [MemberNotNullWhen(returnValue: true, member: nameof(Value))]
    public override bool Succeeded => base.Succeeded;

    [MemberNotNullWhen(returnValue: false, member: nameof(Value))]
    public override bool HasWarnings() => base.HasWarnings();

    [MemberNotNullWhen(returnValue: false, member: nameof(Value))]
    public override bool HasErrors() => base.HasErrors();

    public static OperationResult<T> Success(T value, params OperationResultMessage[] messages) =>
        new SuccessfulOperationResult<T>(value, messages);

    public static OperationResult<T> Failure(T value, params OperationResultMessage[] messages) =>
        new FailedOperationResult<T>(value, messages);

    public new static OperationResult<T> Failure(params OperationResultMessage[] messages) =>
        new FailedOperationResult<T>(default, messages);

    public static implicit operator T(OperationResult<T> result)
    {
        if (result.Value == null)
            throw new InvalidOperationException();

        return result.Value;
    }

    public static implicit operator OperationResult<T>(T? value)
    {
        return value is null
            ? Failure()
            : Success(value);
    }
}