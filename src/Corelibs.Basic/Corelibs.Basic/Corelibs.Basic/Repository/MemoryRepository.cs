using Corelibs.Basic.Blocks;
using Corelibs.Basic.DDD;
using System.Collections.Concurrent;

namespace Corelibs.Basic.Repository;

public class MemoryRepository<TEntity, TEntityId> : IRepository<TEntity, TEntityId>, IMemoryRepository<TEntity, TEntityId>
    where TEntity : IEntity<TEntityId>
    where TEntityId : EntityId
{
    private readonly ConcurrentDictionary<string, EntityData> _entities = new();

    public Task<Result> Clear()
    {
        _entities.Clear();
        return Result.SuccessTask();
    }

    public Task<Result> Delete(TEntityId id)
    {
        _entities.TryRemove(id.Value, out var value);
        return Result.SuccessTask();
    }

    public Task<Result<bool>> ExistsOfName(string name, Func<TEntity, string> getName)
    {
        throw new NotImplementedException();
    }

    public Task<Result<TEntity[]>> GetAll()
    {
        return Task.FromResult(new Result<TEntity[]>(_entities.Values.Select(d => d.Entity).ToArray()));
    }

    public Task<Result<TEntity[]>> GetAll(Action<int> setProgress, CancellationToken ct)
    {
        return Task.FromResult(new Result<TEntity[]>(_entities.Values.Select(d => d.Entity).ToArray()));
    }

    public Task<Result<TEntity>> GetBy(TEntityId id)
    {
        if (!_entities.TryGetValue(id.Value, out var data))
            return Result<TEntity>.FailureTask();

        return Task.FromResult(new Result<TEntity>(data.Entity));
    }

    public Task<Result<TEntity[]>> GetBy(IList<TEntityId> ids)
    {
        return Task.FromResult(new Result<TEntity[]>(ids.Select(id => _entities[id.Value].Entity).ToArray()));
    }

    public Task<Result<TEntity>> GetOfName(string name, Func<TEntity, string> getName)
    {
        throw new NotImplementedException();
    }

    public Task<Result> Save(TEntity item)
    {
        EntityData data;
        if (_entities.ContainsKey(item.Id.Value))
            data = _entities[item.Id.Value];
        else
        {
            data = new(item);
            if (!_entities.TryAdd(item.Id.Value, data))
                return Result.FailureTask();
        }

        lock (data.Lock)
        {
            item.IncrementVersion(ref data.Lock);
            data.Entity = item;
        }

        return Result.SuccessTask();
    }

    public Task<Result> Create(IEnumerable<TEntity> items)
    {
        throw new NotImplementedException();
    }

    public Task<Result> Create(TEntity item)
    {
        throw new NotImplementedException();
    }

    private class EntityData
    {

        public TEntity Entity { get; set; }

        public EntityData(TEntity entity)
        {
            Entity = entity;
        }

        public object Lock = new();
    }
}
