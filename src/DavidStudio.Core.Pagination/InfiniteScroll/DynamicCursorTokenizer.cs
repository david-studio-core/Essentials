using System.Text.Json;

namespace DavidStudio.Core.Pagination.InfiniteScroll;

public static class DynamicCursorTokenizer
{
    public static string Encode(this DynamicCursor cursor)
    {
        var json = JsonSerializer.Serialize(cursor.Values);
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
    }

    public static DynamicCursor? Decode(string? token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(token));
        var jsonElements = JsonSerializer.Deserialize<object?[]>(json) ?? [];

        var values = new object?[jsonElements.Length];
        for (var i = 0; i < jsonElements.Length; i++)
            values[i] = ConvertJsonElement(jsonElements[i]);

        return new DynamicCursor(values);
    }

    private static object? ConvertJsonElement(object? obj)
    {
        if (obj is not JsonElement je) return obj;

        return je.ValueKind switch
        {
            JsonValueKind.Number => je.TryGetInt32(out var i) ? i :
                je.TryGetInt64(out var l) ? l :
                je.TryGetDecimal(out var d) ? d :
                je.GetDouble(),
            JsonValueKind.String => je.GetString(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => throw new NotSupportedException($"Unsupported JSON element {je.ValueKind}")
        };
    }
}