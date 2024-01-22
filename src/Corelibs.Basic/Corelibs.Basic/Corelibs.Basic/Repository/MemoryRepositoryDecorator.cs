using Corelibs.Basic.Blocks;
using Corelibs.Basic.DDD;

namespace Corelibs.Basic.Repository;

public class MemoryRepositoryDecorator<TEntity, TEntityId> : IMemoryRepository<TEntity, TEntityId>, IRepository<TEntity, TEntityId>
    where TEntity : IEntity<TEntityId>
    where TEntityId : EntityId
{
    private readonly IRepository<TEntity, TEntityId> _memoryRepository;
    private readonly IRepository<TEntity, TEntityId> _otherRepository;

    public MemoryRepositoryDecorator(
        IRepository<TEntity, TEntityId> memoryRepository,
        IRepository<TEntity, TEntityId> otherRepository)
    {
        _memoryRepository = memoryRepository;
        _otherRepository = otherRepository;
    }

    Task<Result<TEntity>> IMemoryRepository<TEntity, TEntityId>.GetBy(TEntityId id)
    {
        return _memoryRepository.GetBy(id);
    }

    Task<Result> IMemoryRepository<TEntity, TEntityId>.Save(TEntity item)
    {
        return _memoryRepository.Save(item);
    }

    public Task<Result> Clear()
    {
        return _otherRepository.Clear();
    }

    public Task<Result> Delete(TEntityId id)
    {
        return _otherRepository.Delete(id);
    }

    public Task<Result<bool>> ExistsOfName(string name, Func<TEntity, string> getName)
    {
        return _otherRepository.ExistsOfName(name, getName);
    }

    public Task<Result<TEntity[]>> GetAll()
    {
        return _otherRepository.GetAll();
    }

    public Task<Result<TEntity[]>> GetAll(Action<int> setProgress, CancellationToken ct)
    {
        return _otherRepository.GetAll(setProgress, ct);
    }

    public async Task<Result<TEntity>> GetBy(TEntityId id)
    {
        var result = await _memoryRepository.GetBy(id);
        if (result.IsSuccess)
            return result;

        return await _otherRepository.GetBy(id);
    }

    public Task<Result<TEntity[]>> GetBy(IList<TEntityId> ids)
    {
        return _otherRepository.GetBy(ids);
    }

    public Task<Result<TEntity>> GetOfName(string name, Func<TEntity, string> getName)
    {
        return _otherRepository.GetOfName(name, getName);
    }

    public Task<Result> Save(TEntity item)
    {
        return _otherRepository.Save(item);
    }

    public Task<Result> Create(IEnumerable<TEntity> items)
    {
        return _otherRepository.Create(items);
    }

    public Task<Result> Create(TEntity item)
    {
        return _otherRepository.Create(item);
    }
}
