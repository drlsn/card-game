using Corelibs.Basic.Collections;
using System.Security.Cryptography;
using System.Text;

namespace Corelibs.Basic.Encryption;

public static class EncryptionFunctions
{
    public static string GenerateGuidHash(params string[] args)
    {
        if (args.IsNullOrEmptyOrOne())
            return string.Empty;

        var combinedValue = args.Aggregate((x, y) => x + y);

        using (var sha256 = SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(Encoding.UTF32.GetBytes(combinedValue));

            var guidBytes = new byte[16];
            Array.Copy(hashBytes, guidBytes, 16);

            return new Guid(guidBytes).ToString();
        }
    }
}
