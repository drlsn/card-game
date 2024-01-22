namespace Corelibs.Basic.Files
{
    public interface IFileOperations
    {
        string ReadAsText(string filePath);
        void WriteAsText(string filePath, string text);
        void Delete(string filePath);
    }
}
