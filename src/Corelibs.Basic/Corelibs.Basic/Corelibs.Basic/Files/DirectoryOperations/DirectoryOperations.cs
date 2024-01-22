using System.IO;

namespace Corelibs.Basic.Files
{
    public class DirectoryOperations : IDirectoryOperations
    {
        void IDirectoryOperations.Create(string path) => Directory.CreateDirectory(path);
        bool IDirectoryOperations.Exists(string path) => Directory.Exists(path);
        string[] IDirectoryOperations.GetFiles(string path) => Directory.GetFiles(path);
    }
}
