namespace Trinica.Entities.Gameplay.Events;

public class GameStartedEvent : GameEvent
{
    public GameStartedEvent(GameId gameId) : base(gameId)
    {
    }
}
