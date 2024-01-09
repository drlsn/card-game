using Corelibs.Basic.DDD;
using Mediator;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay.Events;

public abstract class GameEvent : INotification, IDomainEvent
{
    public GameEvent(
        GameId gameId,
        UserId? playerId = null)
    {
        GameId = gameId;
        PlayerId = playerId;
    }

    public GameId GameId { get; }
    public UserId? PlayerId { get; }

    public virtual string ToMessage() => "";

    public string Id => throw new NotImplementedException();
    public long Timestamp => throw new NotImplementedException();
}
