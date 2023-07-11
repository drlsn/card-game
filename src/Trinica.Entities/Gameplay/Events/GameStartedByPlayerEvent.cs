using Mediator;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay.Events;

public class GameStartedByPlayerEvent : INotification
{
    public GameStartedByPlayerEvent(UserId playerId, GameId gameId, GameActionController gameActionController)
    {
        PlayerId = playerId;
        GameId = gameId;
        GameActionController = gameActionController;
    }

    public UserId PlayerId { get; }
    public GameId GameId { get; }
    public GameActionController GameActionController { get; }
}
