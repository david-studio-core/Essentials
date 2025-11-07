using System.Security.Cryptography;
using System.Text;

namespace DavidStudio.Core.Utilities.Utilities;

public static class CodeGenerator
{
    private const string Base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

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
