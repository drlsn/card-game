using Corelibs.Basic.Blocks;
using Corelibs.Basic.DDD;

namespace Corelibs.Basic.Repository;

public interface IMemoryRepository<TEntity, TEntityId>
    where TEntity : IEntity<TEntityId>
    where TEntityId : EntityId
{
    Task<Result<TEntity>> GetBy(TEntityId id);
    Task<Result> Save(TEntity item);
}
