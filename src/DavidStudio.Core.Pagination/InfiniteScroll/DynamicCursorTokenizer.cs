using System.Globalization;
using System.Numerics;
using System.Text.Json;

namespace DavidStudio.Core.Pagination.InfiniteScroll;

/// <summary>
/// Provides helper methods to encode and decode <see cref="DynamicCursor"/> instances
/// to and from a string token suitable for client-side infinite scroll (crusor) pagination.
/// </summary>
public static class DynamicCursorTokenizer
{
    /// <summary>
    /// Encodes a <see cref="DynamicCursor"/> into a Base64 string token.
    /// This token can be used by clients to request the next page.
    /// </summary>
    /// <param name="cursor">The dynamic cursor containing the values of the last item on a page.</param>
    /// <returns>A Base64-encoded string representing the cursor.</returns>
    public static string Encode(this DynamicCursor cursor)
    {
        var json = JsonSerializer.Serialize(cursor.Values);
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
    }

    /// <summary>
    /// Decodes a Base64 string token back into a <see cref="DynamicCursor"/>.
    /// </summary>
    /// <param name="token">The Base64 string token representing a cursor.</param>
    /// <returns>The decoded <see cref="DynamicCursor"/>, or <c>null</c> if the token is <c>null</c> or empty.</returns>
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

    /// <summary>
    /// Converts a <see cref="JsonElement"/> or object to a .NET type.
    /// Handles numbers, strings, booleans, and nulls.
    /// </summary>
    /// <param name="obj">The object or <see cref="JsonElement"/> to convert.</param>
    /// <returns>The converted .NET value.</returns>
    /// <exception cref="NotSupportedException">Thrown if the <see cref="JsonElement"/> type is unsupported.</exception>
    private static object? ConvertJsonElement(object? obj)
    {
        if (obj is not JsonElement je) return obj;

        return je.ValueKind switch
        {
            JsonValueKind.String => ConvertJsonString(je),
            JsonValueKind.Number => ConvertJsonNumber(je),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            JsonValueKind.Undefined => null,
            _ => throw new NotSupportedException($"Unsupported JSON element {je.ValueKind}")
        };
    }

    private static object? ConvertJsonString(JsonElement je)
    {
        var s = je.GetString();

        if (s is null)
            return null;

        if (Guid.TryParse(s, out var guid))
            return guid;

        if (DateTime.TryParse(s, null, DateTimeStyles.RoundtripKind, out var dt))
            return dt;

        if (DateOnly.TryParse(s, out var dateOnly))
            return dateOnly;

        if (TimeOnly.TryParse(s, out var timeOnly))
            return timeOnly;

        if (TimeSpan.TryParse(s, out var ts))
            return ts;

        if (s.Length == 1)
            return s[0];

        return s;
    }

    private static object ConvertJsonNumber(JsonElement je)
    {
        if (je.TryGetByte(out var b)) return b;
        if (je.TryGetSByte(out var sb)) return sb;
        if (je.TryGetInt16(out var s)) return s;
        if (je.TryGetUInt16(out var us)) return us;
        if (je.TryGetInt32(out var i)) return i;
        if (je.TryGetUInt32(out var ui)) return ui;
        if (je.TryGetInt64(out var l)) return l;
        if (je.TryGetUInt64(out var ul)) return ul;

        if (je.TryGetDecimal(out var dec))
            return dec;

        if (je.TryGetDouble(out var dbl))
            return dbl;

        if (BigInteger.TryParse(je.GetRawText(), out var big))
            return big;

        return je.GetRawText();
    }
}