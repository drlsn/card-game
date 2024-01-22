using Corelibs.Basic.Blocks;
using System.Threading.Tasks;

namespace Corelibs.Basic.Files
{
    public interface IFile<T>
        where T : class
    {
        Task<Result<T>> Get();
        Task<Result> Save(T @object);
    }
}
