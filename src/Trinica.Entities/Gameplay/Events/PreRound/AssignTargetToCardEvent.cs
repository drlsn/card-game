using Mediator;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay.Events;

public class AssignTargetToCardEvent : GameEvent, INotification
{
    public AssignTargetToCardEvent(GameId gameId, UserId playerId) : base(gameId, playerId) {}
}
