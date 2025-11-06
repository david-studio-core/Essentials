using System.Linq.Expressions;

namespace DavidStudio.Core.DataIO.Helpers;

public static class ExpressionPropertyHelper
{
    public static bool AllOrderByFieldsExistInSelector<TEntity, TResult>(
        IReadOnlyList<Expression<Func<TEntity, object>>> orderBy,
        Expression<Func<TEntity, TResult>> selector)
    {
        if (selector.Parameters[0] == selector.Body)
            return true;

        var orderByProps = orderBy
            .Select(GetPropertyName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var selectorProps = GetSelectorPropertyNames(selector)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return orderByProps.All(selectorProps.Contains);
    }

    public static string GetPropertyName<TEntity>(Expression<Func<TEntity, object>> expression)
    {
        var body = expression.Body;

        if (body is UnaryExpression { NodeType: ExpressionType.Convert } unary)
            body = unary.Operand;

        if (body is MemberExpression member)
            return member.Member.Name;

        throw new InvalidOperationException($"Unsupported expression: {expression}");
    }

    public static IEnumerable<string> GetSelectorPropertyNames<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> selector)
    {
        return selector.Body switch
        {
            NewExpression newExpr => newExpr.Members?.Select(m => m.Name) ?? [],
            MemberInitExpression initExpr => initExpr.Bindings.Select(b => b.Member.Name),
            MemberExpression member => [member.Member.Name],
            _ => throw new InvalidOperationException($"Unsupported expression: {selector}")
        };
    }
}