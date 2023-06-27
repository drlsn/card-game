using CardGame.Entities.Users;
using Corelibs.Basic.Collections;
using Corelibs.Basic.DDD;

namespace CardGame.Entities.Gameplay;

public class Player : Entity<UserId>
{
    public UserId UserId { get; private set; }
    public EntireDeck Deck { get; private set; }
    public FieldDeck HandCards { get; private set; }

    public FieldDeck ShuffleAndTakeHalfCards(Random random) =>
        Deck.ShuffleAndTakeHalfCards(random);
}

public static class PlayerExtensions
{
    public static FieldDeck ShuffleAndGetHalfCards(this IEnumerable<Player> Players, Random random) =>
        Players
            .Select(player => player.ShuffleAndTakeHalfCards(random))
            .AggregateOrDefault((x, y) => x + y);
}
