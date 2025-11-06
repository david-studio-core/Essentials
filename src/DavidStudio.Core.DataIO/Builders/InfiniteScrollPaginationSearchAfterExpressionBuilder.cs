using System.Linq.Expressions;
using DavidStudio.Core.DataIO.Expressions;

namespace DavidStudio.Core.DataIO.Builders;

public static class InfiniteScrollPaginationSearchAfterExpressionBuilder
{
    public static Expression<Func<TEntity, bool>> Build<TEntity>(
        IReadOnlyList<Expression<Func<TEntity, object>>> orderBy,
        bool[] isDescending,
        object?[] searchAfterValues) where TEntity : class
    {
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        Expression? filter = null;

        for (var i = 0; i < orderBy.Count; i++)
        {
            var orderSelector = orderBy[i];
            var orderMember = UnWrap(orderSelector.Body);
            orderMember = new ReplaceParameterVisitor(orderSelector.Parameters[0], parameter).Visit(orderMember);
            var orderValue = Expression.Constant(searchAfterValues[i]);

            var comparison = BuildComparison(orderMember, orderValue, !isDescending[i]);

            if (filter is null)
                filter = comparison;
            else
            {
                var prevEquals = BuildPreviousEquals(orderBy, i, searchAfterValues, parameter);
                filter = Expression.OrElse(filter, Expression.AndAlso(prevEquals, comparison));
            }
        }

        return Expression.Lambda<Func<TEntity, bool>>(filter!, parameter);
    }

    private static Expression UnWrap(Expression body)
        => body is UnaryExpression { NodeType: ExpressionType.Convert } unary
            ? unary.Operand
            : body;

    private static BinaryExpression BuildComparison(Expression orderMember, Expression orderValue, bool asc)
    {
        if (orderMember.Type == typeof(string))
        {
            var compareMethod = typeof(string).GetMethod(nameof(string.Compare), [typeof(string), typeof(string)])!;
            var compareCall = Expression.Call(compareMethod, orderMember, orderValue);

            return asc
                ? Expression.GreaterThan(compareCall, Expression.Constant(0))
                : Expression.LessThan(compareCall, Expression.Constant(0));
        }

        return asc
            ? Expression.GreaterThan(orderMember, orderValue)
            : Expression.LessThan(orderMember, orderValue);
    }

    private static Expression BuildPreviousEquals<TEntity>(IReadOnlyList<Expression<Func<TEntity, object>>> orderBy,
        int upToIndex,
        object?[] searchAfterValues,
        ParameterExpression parameter)
    {
        Expression? prevEquals = null;

        for (var j = 0; j < upToIndex; j++)
        {
            var prevSelector = orderBy[j];
            var prevMember = Expression.Invoke(prevSelector, parameter);
            var prevValue = Expression.Convert(
                Expression.Constant(searchAfterValues[j]),
                prevMember.Type
            );

            var eq = Expression.Equal(prevMember, prevValue);
            prevEquals = prevEquals == null ? eq : Expression.AndAlso(prevEquals, eq);
        }

        return prevEquals!;
    }
}