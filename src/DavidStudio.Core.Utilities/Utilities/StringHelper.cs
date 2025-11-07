namespace DavidStudio.Core.Utilities.Utilities;

public class StringHelper
{
    public static string FirstCharToLower(string input)
    {
        if (string.IsNullOrEmpty(input) || char.IsLower(input[0]))
            return input;

        return char.ToLower(input[0]) + input[1..];
    }
}