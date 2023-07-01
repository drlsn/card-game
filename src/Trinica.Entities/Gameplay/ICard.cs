using Corelibs.Basic.DDD;

namespace Trinica.Entities.Gameplay;

public interface ICard<TId>
    where TId : EntityId
{
    TId Id { get; }
}
