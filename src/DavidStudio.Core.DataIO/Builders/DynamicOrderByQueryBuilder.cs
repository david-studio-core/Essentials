using System.Linq.Expressions;
using System.Text;

namespace DavidStudio.Core.DataIO.Builders;

public static class DynamicOrderByQueryBuilder
{
    public static string BuildString(string orderBy)
    {
        var orderParams = orderBy.Trim().Split(',');
        var orderQueryBuilder = new StringBuilder();

        foreach (var param in orderParams)
        {
            if (string.IsNullOrWhiteSpace(param))
                continue;

            var orderByProperty = param.Split(' ')[0];
            var direction = param.EndsWith(" desc") ? "descending" : "ascending";

            orderQueryBuilder.Append($"{orderByProperty} {direction}, ");
        }

        var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');

        return orderQuery;
    }

    public static (IReadOnlyList<Expression<Func<TEntity, object>>> orderBy, bool[] isDescending)
        BuildInfiniteScrollPaginationObjects<TEntity>(string orderBy)
    {
        List<Expression<Func<TEntity, object>>> orderByExpressions = [];
        List<bool> isDescending = [];

        var orderParams = orderBy.Trim().Split(',');

        foreach (var param in orderParams)
        {
            if (string.IsNullOrWhiteSpace(param))
                continue;

            var orderByProperty = param.Split(' ')[0];
            
            

            isDescending.Add(param.EndsWith(" desc"));
        }

        return (orderByExpressions, isDescending.ToArray());
    }
}