using System.Text.RegularExpressions;

namespace Corelibs.Basic.Identity;

public static class IdCreator
{
    public static string CreateBase64GuidId() =>
        Regex.Replace(
            Convert.ToBase64String(
                Guid.NewGuid().ToByteArray()),
            "[^a-zA-Z0-9]",
            "");
}
