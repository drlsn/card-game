using Mediator;

namespace Corelibs.Basic.DDD;

public interface IEntity
{
    uint Version { get; set; }
    List<IDomainEvent> DomainEvents { get; }
}

public interface IEntity<TId> : IEntity
    where TId : EntityId
{
    TId Id { get; }
}
