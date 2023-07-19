using Mediator;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay.Events;

public class DicesReplayPassedEvent : GameEvent, INotification
{
    public DicesReplayPassedEvent(GameId gameId, UserId playerId) : base(gameId, playerId) {}
}
