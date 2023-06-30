using CardGame.Entities.Users;
using Corelibs.Basic.Collections;
using Corelibs.Basic.DDD;
using Corelibs.Basic.Maths;

namespace CardGame.Entities.Gameplay;

public class Player : Entity<UserId>
{
    public const int PlayableCardsPerPlayerCount = 30;
    public const int MaxHandCardsCount = 6;

    public UserId UserId { get; private set; }
    public EntireDeck FloorDeck { get; private set; }
    public FieldDeck HandDeck { get; private set; }

    public FieldDeck ShuffleAllAndTakeHalfCards(Random random)
    {
        FloorDeck.ShuffleAll(random);
        return FloorDeck.TakeNCards(random, PlayableCardsPerPlayerCount / 2);
    }

    public void TakeNCardsToHand(Random random, int n)
    {
        var maxCardsCanTakeCount = MaxHandCardsCount - HandDeck.Count;
        n = n.Clamp(0, maxCardsCanTakeCount);
        HandDeck = FloorDeck.TakeNCards(random, n);
    }
}

public static class PlayerExtensions
{
    public static FieldDeck ShuffleAllAndTakeHalfCards(this IEnumerable<Player> Players, Random random) =>
        Players
            .Select(player => player.ShuffleAllAndTakeHalfCards(random))
            .AggregateOrDefault((x, y) => x + y);

    public static void TakeNCardsToHand(this IEnumerable<Player> Players, Random random, int n = Player.MaxHandCardsCount) =>
        Players.ForEach(player => player.TakeNCardsToHand(random, n));
            
}
