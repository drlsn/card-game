using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay.Events;

public class CardsLayPassedByPlayerEvent : GameEvent
{
    public CardsLayPassedByPlayerEvent(
        GameId gameId,
        UserId playerId,
        PlayerData[] players) : base(gameId, playerId)
    {
        Players = players;
    }

    public PlayerData[] Players { get; }
}
