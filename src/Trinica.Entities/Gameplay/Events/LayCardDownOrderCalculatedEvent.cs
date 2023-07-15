using Mediator;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay.Events;

public class LayCardDownOrderCalculatedEvent : GameEvent
{
    public LayCardDownOrderCalculatedEvent(
        GameId gameId,
        UserId[] playerIds) : base(gameId)
    {
        PlayerIds = playerIds;
    }

    public UserId[] PlayerIds { get; }
}
