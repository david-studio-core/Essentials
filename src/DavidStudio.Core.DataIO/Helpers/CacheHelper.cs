using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace DavidStudio.Core.DataIO.Helpers;

internal static class CacheHelper
{
    private static readonly ConcurrentDictionary<string, Delegate> CompiledExpressionsCache = new();
    private static readonly ConcurrentDictionary<Type, HashSet<string>> TypePropertiesCache = new();

    public static Func<TEntity, TReadDto> CompileToReadDtoExpression<TEntity, TReadDto>(
        Expression<Func<TEntity, TReadDto>> expression)
    {
        return (Func<TEntity, TReadDto>)CompiledExpressionsCache.GetOrAdd(
            expression.ToString(),
            _ => expression.Compile());
    }

    public static HashSet<string> GetTypeProperties<T>()
    {
        return TypePropertiesCache.GetOrAdd(
            typeof(T),
            t => t.GetProperties().Select(p => p.Name).ToHashSet(StringComparer.OrdinalIgnoreCase));
    }
}