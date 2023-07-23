using Mediator;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay.Events;

public class AssignTargetsToCardConfirmedEvent : GameEvent, INotification
{
    public AssignTargetsToCardConfirmedEvent(GameId gameId, UserId playerId) : base(gameId, playerId) {}
}
