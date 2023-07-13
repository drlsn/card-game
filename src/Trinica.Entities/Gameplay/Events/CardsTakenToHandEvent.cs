using Mediator;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay.Events;

public class CardsTakenToHandEvent : INotification
{
    public CardsTakenToHandEvent(
        UserId playerId,
        GameId gameId)
    {
        PlayerId = playerId;
        GameId = gameId;
    }

    public UserId PlayerId { get; }
    public GameId GameId { get; }
}
