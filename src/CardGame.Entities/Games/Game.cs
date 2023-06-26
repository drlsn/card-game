using CardGame.Entities.Decks;
using Corelibs.Basic.DDD;

namespace CardGame.Entities.Gameplay;

public record GameId(string Value) : EntityId(Value);

public class Game : Entity<GameId>
{
    public Player[] Players { get; private set; }
    public Deck[] Decks { get; private set; }
    public Deck CommonPool { get; private set; }

    public Game(
        GameId id, 
        Deck[] decks) : base(id)
    {
        Decks = decks;
    }

    public void TakeHalfCardsToCommonPool(object random)
    {
        var deckRandomHalf = Players.Select(player =>
        {
            // player.Deck.Shuffle(random);
            return default(Deck);// player.Deck.TakeHalf(random);
        });

        // CommonPool = deckRandomHalf;

        // player.Cards.TakeRandomHalf(random)
        // player.Cards.TakeRandomHalf(random)
    }

    public void DrawCardsToHand()
    {
        var deckRandomHalf = Players.Select(player =>
        {
            // player.Deck.Shuffle(random);
            return default(Deck);// player.Deck.TakeHalf(random);
        });

        // CommonPool = deckRandomHalf;

        // player.Cards.TakeRandomHalf(random)
        // player.Cards.TakeRandomHalf(random)
    }

    public void CalculateRoundPlayerOrder()
    {
        var deckRandomHalf = Players.Select(player =>
        {
            // player.GetOverallSpeed();
            return default(Deck);// player.Deck.TakeHalf(random);
        });

        // CommonPool = deckRandomHalf;

        // player.Cards.TakeRandomHalf(random)
        // player.Cards.TakeRandomHalf(random)
    }
}

public record PlayerId(string Value) : EntityId(Value);
public class Player : Entity<PlayerId>
{
    public Deck Deck { get; private set; }
    public object HandCards { get; private set; }
}
