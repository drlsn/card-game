using Mediator;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay.Events;

public class LayCardDownOrderCalculatedEvent : INotification
{
    public LayCardDownOrderCalculatedEvent(
        GameId gameId,
        UserId[] playerIds)
    {
        GameId = gameId;
        PlayerIds = playerIds;
    }

    public GameId GameId { get; }
    public UserId[] PlayerIds { get; }
}
