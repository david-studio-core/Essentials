using System.Linq.Expressions;
using DavidStudio.Core.Results;

namespace DavidStudio.Core.DataIO.Helpers;

/// <summary>
/// Provides helper methods for dynamic sorting.
/// </summary>
public static class DynamicOrderingHelper
{
    /// <summary>
    /// Validates that the specified <paramref name="orderBy"/> string contains only valid
    /// and allowed properties for the given entity type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The entity type whose properties are being validated for ordering.
    /// </typeparam>
    /// <param name="orderBy">
    /// A comma-separated list of property names (optionally followed by sort direction)
    /// used to define ordering, for example: <c>"Name desc, Year asc"</c>.
    /// It also can have nested property paths, for example: <c>ManufacturingInfo.Address.City</c>.
    /// All properties should follow <c>PascalCase</c> style. This is a case-sensitive argument.
    /// </param>
    /// <param name="allowedProperties">
    /// An optional list of property expressions that define which entity properties are allowed
    /// to be used for ordering. If <see langword="null"/>, all entity properties are considered valid.
    /// </param>
    /// <returns>
    /// An <see cref="OperationResult"/> indicating whether the provided <paramref name="orderBy"/> parameters
    /// are valid.  
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// Returns <see cref="OperationResult.Success(OperationResultMessage[])"/> if all ordering parameters are valid.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Returns <see cref="OperationResult.Failure(OperationResultMessage[])"/> if any parameter references
    /// a property that is not allowed or does not exist in <typeparamref name="TEntity"/>.
    /// </description>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <para>
    /// All properties should follow <c>PascalCase</c> style in <paramref name="orderBy"/> argument. This is a case-sensitive argument.
    /// </para>
    /// <para>
    /// The method trims and splits the <paramref name="orderBy"/> string by commas, then checks each
    /// ordering field name against the list of allowed properties or the entity's actual properties.
    /// </para>
    /// <para>
    /// If <paramref name="allowedProperties"/> is not <see langword="null"/>, validation is restricted
    /// to those explicitly provided expressions. Otherwise, the check is performed using all properties
    /// retrieved from <see cref="TEntity"/> and cached in memory.
    /// </para>
    /// <para>
    /// If a field is invalid, the result contains an error message describing the invalid field.
    /// </para>
    /// </remarks>
    public static OperationResult Validate<TEntity>(string orderBy,
        IReadOnlyList<Expression<Func<TEntity, object>>>? allowedProperties)
    {
        var orderParams = orderBy.Trim().Split(',');
        var entityProps = allowedProperties is null ? CacheHelper.GetTypeProperties<TEntity>() : null;

        foreach (var param in orderParams)
        {
            if (string.IsNullOrWhiteSpace(param))
                continue;

            var orderingProperty = param.Split(" ")[0];

            if (allowedProperties is not null)
            {
                var propertyAllowed =
                    allowedProperties.Any(a => ExpressionsHelper.GetPropertyPath(a) == orderingProperty);

                if (!propertyAllowed)
                {
                    return OperationResult.Failure(
                        new OperationResultMessage($"Ordering parameter '{param}' is not allowed.",
                            OperationResultSeverity.Error));
                }
            }
            else
            {
                var fieldExists = entityProps!.Contains(orderingProperty);
                if (!fieldExists)
                    return OperationResult.Failure(
                        new OperationResultMessage($"Field '{param}' does not not exist.",
                            OperationResultSeverity.Error));
            }
        }

        return OperationResult.Success();
    }

    /// <summary>
    /// Applies multiple ordering expressions to the specified <see cref="IQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entities in the query.</typeparam>
    /// <param name="query">The query to apply ordering to.</param>
    /// <param name="orderBy">A list of expressions specifying the properties to order by.</param>
    /// <param name="isDescending">A boolean array indicating for each expression whether the ordering should be descending.</param>
    /// <returns>An <see cref="IOrderedQueryable{TEntity}"/> representing the ordered query.</returns>
    /// <remarks>
    /// This method supports multi-level ordering: the first expression determines the primary order,
    /// subsequent expressions are applied as secondary, tertiary, etc. ordering using <c>ThenBy</c> or <c>ThenByDescending</c>.
    /// </remarks>
    public static IOrderedQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> query,
        IReadOnlyList<Expression<Func<TEntity, object>>> orderBy,
        bool[] isDescending)
    {
        IOrderedQueryable<TEntity> ordered = null!;

        for (var i = 0; i < orderBy.Count; i++)
        {
            if (i == 0)
            {
                ordered = !isDescending[i]
                    ? query.OrderBy(orderBy[i])
                    : query.OrderByDescending(orderBy[i]);
            }
            else
            {
                ordered = !isDescending[i]
                    ? ordered.ThenBy(orderBy[i])
                    : ordered.ThenByDescending(orderBy[i]);
            }
        }

        return ordered;
    }
}