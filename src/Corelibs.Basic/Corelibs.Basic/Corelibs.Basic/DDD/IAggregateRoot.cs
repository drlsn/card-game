namespace Corelibs.Basic.DDD;

public interface IAggregateRoot<TId> : IAggregateRoot, IEntity<TId>
    where TId : EntityId
{
    public static abstract string DefaultCollectionName { get; }
}

public interface IAggregateRoot
{
}
