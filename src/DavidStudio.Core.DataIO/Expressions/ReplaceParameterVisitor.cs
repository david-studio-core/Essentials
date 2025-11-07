using System.Linq.Expressions;

namespace DavidStudio.Core.DataIO.Expressions;

/// <summary>
/// An <see cref="ExpressionVisitor"/> that replaces occurrences of a specified <see cref="ParameterExpression"/>
/// with another <see cref="ParameterExpression"/> within an expression tree.
/// </summary>
/// <remarks>
/// This is useful when combining lambda expressions that have different parameter instances
/// but represent the same logical parameter. It allows dynamically rewriting expression trees
/// to unify parameter references.
/// </remarks>
public class ReplaceParameterVisitor(ParameterExpression oldParam, ParameterExpression newParam)
    : ExpressionVisitor
{
    protected override Expression VisitParameter(ParameterExpression node)
        => node == oldParam ? newParam : base.VisitParameter(node);
}