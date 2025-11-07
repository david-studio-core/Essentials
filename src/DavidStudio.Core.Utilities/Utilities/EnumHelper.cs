namespace DavidStudio.Core.Utilities.Utilities;

public static class EnumHelper
{
    public static TEnum ToEnum<TEnum>(this string value, TEnum defaultValue) where TEnum : struct
    {
        if (string.IsNullOrEmpty(value))
            return defaultValue;

        return Enum.TryParse<TEnum>(value, true, out var result) ? result : defaultValue;
    }

    public static TEnum ToEnum<TEnum>(this string value) where TEnum : struct
    {
        return Enum.Parse<TEnum>(value, true);
    }
}