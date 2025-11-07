using System.Security.Cryptography;
using System.Text;

namespace DavidStudio.Core.Utilities.Utilities;

/// <summary>
/// Provides functionality to generate random alphanumeric codes using a Base62 character set.
/// </summary>
/// <remarks>
/// The generated codes contain characters from the set: 0-9, A-Z, a-z.
/// Uses a cryptographically secure random number generator (<see cref="RandomNumberGenerator"/>) to ensure unpredictability.
/// </remarks>
public static class CodeGenerator
{
    private const string Base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    /// <summary>
    /// Generates a random alphanumeric code of the specified length.
    /// </summary>
    /// <param name="length">The desired length of the generated code. Defaults to 8.</param>
    /// <returns>A random alphanumeric string composed of characters from 0-9, A-Z, a-z.</returns>
    /// <remarks>
    /// This method uses a cryptographically secure random number generator to produce random bytes.
    /// Each byte is mapped to a character in the Base62 character set by taking the modulo of the character set length.
    /// </remarks>
    public static string GenerateCode(int length = 8)
    {
        var randomBytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        var sb = new StringBuilder(length);
        foreach (var b in randomBytes)
            sb.Append(Base62Chars[b % Base62Chars.Length]);

        return sb.ToString();
    }
}
