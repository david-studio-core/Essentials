using System.Linq.Expressions;

namespace DavidStudio.Core.DataIO.Helpers;

public static class ExpressionPropertyHelper
{
    public static string GetPropertyPath<TEntity>(Expression<Func<TEntity, object>> expression)
    {
        var body = expression.Body;

        if (body is UnaryExpression { NodeType: ExpressionType.Convert } unary)
            body = unary.Operand;

        if (body is not MemberExpression member)
            throw new InvalidOperationException($"Unsupported expression: {expression}");

        var members = new Stack<string>();
        var currentMember = member;

        while (currentMember is not null)
        {
            members.Push(currentMember.Member.Name);
            currentMember = currentMember.Expression as MemberExpression;
        }

        return string.Join('.', members);
    }
}