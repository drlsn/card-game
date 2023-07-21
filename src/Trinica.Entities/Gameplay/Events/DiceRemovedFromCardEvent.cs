using Mediator;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay.Events;

public class DiceRemovedFromCardEvent : GameEvent, INotification
{
    public DiceRemovedFromCardEvent(GameId gameId, UserId playerId) : base(gameId, playerId) {}
}
