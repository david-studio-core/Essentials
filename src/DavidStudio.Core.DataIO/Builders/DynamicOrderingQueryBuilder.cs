using System.Linq.Expressions;

namespace DavidStudio.Core.DataIO.Builders;

public static class DynamicOrderingQueryBuilder
{
    // TODO: Remove it after testing my own method of dynamic ordering 
    //
    // public static string BuildString(string orderBy)
    // {
    //     var orderParams = orderBy.Trim().Split(',');
    //     var orderQueryBuilder = new StringBuilder();
    //
    //     foreach (var param in orderParams)
    //     {
    //         if (string.IsNullOrWhiteSpace(param))
    //             continue;
    //
    //         var orderByProperty = param.Split(' ')[0];
    //         var direction = param.EndsWith(" desc") ? "descending" : "ascending";
    //
    //         orderQueryBuilder.Append($"{orderByProperty} {direction}, ");
    //     }
    //
    //     var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
    //
    //     return orderQuery;
    // }

    public static (IReadOnlyList<Expression<Func<TEntity, object>>> orderBy, bool[] isDescending) Build<TEntity>(
        string orderBy)
    {
        List<Expression<Func<TEntity, object>>> orderByExpressions = [];
        List<bool> isDescending = [];

        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var orderParams = orderBy.Trim().Split(',').Select(p => p.Trim());

        foreach (var param in orderParams)
        {
            if (string.IsNullOrWhiteSpace(param))
                continue;

            var orderByProperty = param.Split(' ')[0];

            var propertyExpression = orderByProperty
                .Split('.')
                .Aggregate<string?, Expression>(parameter, Expression.PropertyOrField!);

            var propertyExpressionObject = Expression.Convert(propertyExpression, typeof(object));
            var lambda = Expression.Lambda<Func<TEntity, object>>(propertyExpressionObject, parameter);

            orderByExpressions.Add(lambda);
            isDescending.Add(param.EndsWith(" desc"));
        }

        return (orderByExpressions, isDescending.ToArray());
    }
}