namespace Corelibs.Basic.Storage;

public static class Files
{
    public static bool IsFilename(string text) =>
        Path.GetFileName(text).Equals(text);
}
