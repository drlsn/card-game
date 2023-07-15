using Mediator;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay.Events;

public abstract class GameEvent : INotification
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
}
