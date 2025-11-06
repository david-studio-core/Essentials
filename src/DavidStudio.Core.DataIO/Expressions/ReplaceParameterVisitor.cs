using System.Linq.Expressions;

namespace DavidStudio.Core.DataIO.Expressions;

public class ReplaceParameterVisitor(ParameterExpression oldParam, ParameterExpression newParam)
    : ExpressionVisitor
{
    protected override Expression VisitParameter(ParameterExpression node)
        => node == oldParam ? newParam : base.VisitParameter(node);
}