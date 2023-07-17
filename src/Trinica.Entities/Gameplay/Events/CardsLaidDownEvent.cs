using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay.Events;

public class CardsLaidDownEvent : GameEvent
{
    public CardsLaidDownEvent(
        GameId gameId,
        UserId playerId,
        CardToLay[] cards,
        bool canLayMore,
        PlayerData[] players) : base(gameId, playerId)
    {
        Cards = cards;
        CanLayMore = canLayMore;
        Players = players;
    }

    public CardToLay[] Cards { get; }
    public bool CanLayMore { get; }
    public PlayerData[] Players { get; }
}
