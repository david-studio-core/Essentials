using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace DavidStudio.Core.DataIO.Helpers;

/// <summary>
/// Provides caching in memory caching of internal library resources to improve performance in repeated operations.
/// </summary>
public static class CacheHelper
{
    private static readonly ConcurrentDictionary<string, Delegate> CompiledExpressionsCache = new();
    private static readonly ConcurrentDictionary<Type, HashSet<string>> TypePropertiesCache = new();

    /// <summary>
    /// Compiles an expression of type <see cref="Expression{Func{TEntity, TReadDto}}"/> into a delegate
    /// and caches it for subsequent use.
    /// </summary>
    /// <typeparam name="TEntity">The type of the source entity.</typeparam>
    /// <typeparam name="TReadDto">The type of the DTO to project to.</typeparam>
    /// <param name="expression">The expression to compile and cache.</param>
    /// <returns>A compiled delegate representing the projection from <typeparamref name="TEntity"/> to <typeparamref name="TReadDto"/>.</returns>
    /// <remarks>
    /// The expression is cached using <see cref="Expression.ToString"/> as the key.
    /// This ensures that repeated calls with the same expression instance do not incur
    /// the cost of recompiling.
    /// </remarks>
    public static Func<TEntity, TReadDto> CompileToReadDtoExpression<TEntity, TReadDto>(
        Expression<Func<TEntity, TReadDto>> expression)
    {
        return (Func<TEntity, TReadDto>)CompiledExpressionsCache.GetOrAdd(
            expression.ToString(),
            _ => expression.Compile());
    }

    /// <summary>
    /// Retrieves a set of property names for a given type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to retrieve property names for.</typeparam>
    /// <returns>A <see cref="HashSet{String}"/> containing all property names of <typeparamref name="T"/>, ignoring case.</returns>
    /// <remarks>
    /// Property names are cached per type to avoid repeated reflection and improve performance.
    /// </remarks>
    public static HashSet<string> GetTypeProperties<T>()
    {
        return TypePropertiesCache.GetOrAdd(
            typeof(T),
            t => t.GetProperties().Select(p => p.Name).ToHashSet(StringComparer.OrdinalIgnoreCase));
    }
}