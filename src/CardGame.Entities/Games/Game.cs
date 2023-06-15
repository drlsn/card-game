using CardGame.Entities.Decks;
using CardGame.Entities.Shared;

namespace CardGame.Entities.Gameplay;

public record GameId(string Value);

public class Game : Entity<GameId>
{
    private readonly Deck[] _decks;

    public Game(GameId id, Deck[] decks) : base(id)
    {
        _decks = decks;
    }

    public void TakeHalfCardsToCommonPool()
    {

    }
}
