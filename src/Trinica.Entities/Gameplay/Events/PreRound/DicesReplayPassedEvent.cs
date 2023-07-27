using Mediator;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay.Events;

public class DiceAssignedToCardEvent : GameEvent, INotification
{
    public DiceAssignedToCardEvent(GameId gameId, UserId playerId) : base(gameId, playerId) {}
}
