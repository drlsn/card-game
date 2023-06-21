using CardGame.Entities.Decks;
using Corelibs.Basic.DDD;

namespace CardGame.Entities.Gameplay;

public record GameId(string Value) : EntityId(Value);

public class Game : Entity<GameId>
{
    public Deck[] Decks { get; private set; }

    public Game(
        GameId id, 
        Deck[] decks) : base(id)
    {
        Decks = decks;
    }

    public void TakeHalfCardsToCommonPool()
    {

    }
}
