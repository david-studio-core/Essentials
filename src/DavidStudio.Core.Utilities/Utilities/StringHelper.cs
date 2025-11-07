namespace DavidStudio.Core.Utilities.Utilities;

/// <summary>
/// Provides helper methods for string manipulation.
/// </summary>
public class StringHelper
{
    /// <summary>
    /// Converts the first character of the input string to lowercase.
    /// </summary>
    /// <param name="input">The input string to process.</param>
    /// <returns>
    /// The input string with the first character converted to lowercase.
    /// If the string is null, empty, or the first character is already lowercase, returns the original string.
    /// </returns>
    /// <remarks>
    /// Useful for converting property names to camelCase or for naming conventions where the first letter should be lowercase.
    /// </remarks>
    public static string FirstCharToLower(string input)
    {
        if (string.IsNullOrEmpty(input) || char.IsLower(input[0]))
            return input;

        return char.ToLower(input[0]) + input[1..];
    }
}