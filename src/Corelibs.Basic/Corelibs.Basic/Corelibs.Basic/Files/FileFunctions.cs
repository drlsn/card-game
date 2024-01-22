namespace Corelibs.Basic.Files
{
    public static class FileFunctions
    {
        public static string CreateFilePath(string pathToDirectory, string name)
        {
            return pathToDirectory.Trim('\\', '/') + '/' + name;
        }
    }
}
