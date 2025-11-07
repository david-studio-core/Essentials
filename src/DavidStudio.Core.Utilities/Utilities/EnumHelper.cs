namespace DavidStudio.Core.Utilities.Utilities;

/// <summary>
/// Provides extension methods for converting strings to enum values.
/// </summary>
/// <remarks>
/// Supports parsing strings into any enum type, optionally specifying a default value if parsing fails.
/// </remarks>
public static class EnumHelper
{
    /// <summary>
    /// Converts a string to an enum of type <typeparamref name="TEnum"/>, returning a default value if conversion fails.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to convert to. Must be a value type.</typeparam>
    /// <param name="value">The string value to convert.</param>
    /// <param name="defaultValue">The default value to return if parsing fails or the string is null/empty.</param>
    /// <returns>The parsed enum value if successful; otherwise, <paramref name="defaultValue"/>.</returns>
    /// <remarks>
    /// The parsing is case-insensitive. If <paramref name="value"/> is null or empty, the <paramref name="defaultValue"/> is returned.
    /// </remarks>
    public static TEnum ToEnum<TEnum>(this string value, TEnum defaultValue) where TEnum : struct
    {
        if (string.IsNullOrEmpty(value))
            return defaultValue;

        return Enum.TryParse<TEnum>(value, true, out var result) ? result : defaultValue;
    }

    /// <summary>
    /// Converts a string to an enum of type <typeparamref name="TEnum"/>. Throws an exception if conversion fails.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to convert to. Must be a value type.</typeparam>
    /// <param name="value">The string value to convert.</param>
    /// <returns>The parsed enum value.</returns>
    /// <exception cref="ArgumentException">Thrown if the string cannot be parsed to the enum type.</exception>
    /// <remarks>
    /// The parsing is case-insensitive. Use this method when you expect the string to always match a valid enum value.
    /// </remarks>
    public static TEnum ToEnum<TEnum>(this string value) where TEnum : struct
    {
        return Enum.Parse<TEnum>(value, true);
    }
}