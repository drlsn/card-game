using Mediator;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay.Events;

public class AssignsDicesToCardConfirmedEvent : GameEvent, INotification
{
    public AssignsDicesToCardConfirmedEvent(GameId gameId, UserId playerId) : base(gameId, playerId) {}
}
