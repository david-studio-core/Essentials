using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;

namespace DavidStudio.Core.DataIO.Extensions;

public static class DistributedCacheExtensions
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = null,
        WriteIndented = true,
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static Task SetAsync<T>(this IDistributedCache cache, string key, T value,
        DistributedCacheEntryOptions? options = null)
    {
        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, SerializerOptions));

        return options is null
            ? cache.SetAsync(key, bytes)
            : cache.SetAsync(key, bytes, options);
    }

    public static async Task<T?> GetAsync<T>(this IDistributedCache cache, string key)
    {
        var val = await cache.GetAsync(key);

        return val == null
            ? default
            : JsonSerializer.Deserialize<T>(val, SerializerOptions);
    }

    public static async Task<T?> GetOrSetAsync<T>(this IDistributedCache cache, string key, Func<Task<T>> task,
        DistributedCacheEntryOptions? options = null)
    {
        var value = await cache.GetAsync<T>(key);
        if (value is not null)
        {
            return value;
        }

        value = await task();
        if (value is not null)
        {
            await cache.SetAsync<T>(key, value, options);
        }

        return value;
    }
}