using Mediator;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay.Events;

public class CardsTakenToHandEvent : GameEvent
{
    public CardsTakenToHandEvent(
        UserId playerId,
        GameId gameId) : base(gameId, playerId)
    {
    }
}
