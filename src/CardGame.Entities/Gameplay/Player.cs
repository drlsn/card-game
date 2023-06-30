using CardGame.Entities.Users;
using Corelibs.Basic.Collections;
using Corelibs.Basic.DDD;

namespace CardGame.Entities.Gameplay;

public class Player : Entity<UserId>
{
    public UserId UserId { get; private set; }
    public EntireDeck Deck { get; private set; }
    public FieldDeck HandCards { get; private set; }

    public FieldDeck ShuffleAllAndTakeHalfCards(Random random) =>
        Deck.ShuffleAllAndTakeHalfCards(random);

    public FieldDeck ShuffleAllAndTakeHalfCards(Random random, int n) =>
        Deck.ShuffleAllAndTakeHalfCards(random);
}

public static class PlayerExtensions
{
    public static FieldDeck ShuffleAllAndTakeHalfCards(this IEnumerable<Player> Players, Random random) =>
        Players
            .Select(player => player.ShuffleAllAndTakeHalfCards(random))
            .AggregateOrDefault((x, y) => x + y);

    public static FieldDeck TakeAndShuffleNCards(this IEnumerable<Player> Players, Random random, int n) =>
        Players
            .Select(player => player.ShuffleAllAndTakeHalfCards(random))
            .AggregateOrDefault((x, y) => x + y);
}
