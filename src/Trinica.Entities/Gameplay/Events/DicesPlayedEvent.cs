using Mediator;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay.Events;

public class DicesPlayedEvent : GameEvent, INotification
{
    public DicesPlayedEvent(GameId gameId, UserId playerId) : base(gameId, playerId) {}
}
