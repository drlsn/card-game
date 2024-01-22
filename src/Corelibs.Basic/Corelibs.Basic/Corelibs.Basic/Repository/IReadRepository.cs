using Corelibs.Basic.Blocks;

namespace Corelibs.Basic.Repository
{
    public interface IReadRepository<TEntityId>
    {
        Task GetBy(TEntityId id, Result result);
    }
}
