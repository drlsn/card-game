using CardGame.Entities.Decks;
using CardGame.Entities.Shared;

namespace CardGame.Entities.Gameplay;

public record GameId(string Value) : Id<GameId>(Value);

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
