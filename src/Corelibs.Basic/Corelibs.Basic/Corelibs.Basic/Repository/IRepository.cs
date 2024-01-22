using Corelibs.Basic.Blocks;

namespace Corelibs.Basic.Repository;

public interface IRepository<TEntity, TEntityId> : IRepository
{
    /// <summary>
    /// Get resource by identification string.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Success result with given type of resource. If none exists then result is also a success, but with no resource inside. If unexpected failure, then false.</returns>
    Task<Result<TEntity>> GetBy(TEntityId id);
    Task<Result<TEntity[]>> GetBy(IList<TEntityId> ids);
    Task<Result<TEntity>> GetOfName(string name, Func<TEntity, string> getName);
    Task<Result<TEntity[]>> GetAll();
    Task<Result<TEntity[]>> GetAll(Action<int> setProgress, CancellationToken ct);
    Task<Result> Save(TEntity item);
    Task<Result> Create(TEntity item);
    Task<Result> Create(IEnumerable<TEntity> items);
    Task<Result> Clear();
    Task<Result> Delete(TEntityId id);

    Task<Result<bool>> ExistsOfName(string name, Func<TEntity, string> getName);
}

public interface IRepository {}
