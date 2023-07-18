using Trinica.Entities.Gameplay.Cards;

namespace Trinica.Entities.Gameplay.Events;

public class LayCardDownOrderCalculatedEvent : GameEvent
{
    public LayCardDownOrderCalculatedEvent(
        GameId gameId,
        PlayerData[] players) : base(gameId)
    {
        Players = players;
    }

    public PlayerData[] Players { get; }
}

public static class LayCardDownOrderCalculatedEventExtensions
{
    public static PlayerData[] ToPlayerData(this Player[] players, Func<ICard, string> toTypeStringFunc) =>
        players.Select(p => new PlayerData(
            p.Id,
            p.HandDeck.GetCards().Select(c => new CardData(c.Id, toTypeStringFunc(c))).ToArray(),
            p.BattlingDeck.GetCards().Select(c => new CardData(c.Id, toTypeStringFunc(c))).ToArray()))
        .ToArray();
}
