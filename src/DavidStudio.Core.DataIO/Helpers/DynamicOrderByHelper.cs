using System.Linq.Expressions;

namespace DavidStudio.Core.DataIO.Helpers;

public static class DynamicOrderByHelper
{
    public static void EnsureOrderByAllowed<TEntity>(string orderBy,
        IReadOnlyList<Expression<Func<TEntity, object>>> allowedProperties)
    {
        var orderParams = orderBy.Trim().Split(',');

        foreach (var param in orderParams)
        {
            if (string.IsNullOrWhiteSpace(param))
                continue;

            var orderingProperty = param.Split(" ")[0];

            var propertyAllowed = allowedProperties.Any(a =>
            {
                var propertyName = ExpressionPropertyHelper.GetPropertyName(a);
                var normalized = char.ToLowerInvariant(propertyName[0]) + propertyName[1..];
                return normalized == orderingProperty;
            });

            if (!propertyAllowed)
                throw new NotSupportedException($"Ordering parameter '{param}' is not allowed.");
        }
    }
}