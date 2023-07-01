using CardGame.Entities.Decks;
using CardGame.Entities.Gameplay.Cards;
using CardGame.Entities.Users;
using Corelibs.Basic.Collections;
using Corelibs.Basic.DDD;
using Corelibs.Basic.Maths;

namespace CardGame.Entities.Gameplay;

public class Player : Entity<UserId>
{
    public const int PlayableCardsPerPlayerCount = 30;
    public const int MaxHandCardsCount = 6;

    public DeckId DeckId { get; private set; }
    public HeroCard HeroCard { get; private set; }
    public FieldDeck IdleDeck { get; private set; }
    public FieldDeck HandDeck { get; private set; }
    public FieldDeck BattlingDeck { get; private set; }

    public FieldDeck ShuffleAllAndTakeHalfCards(Random random)
    {
        IdleDeck.ShuffleAll(random);
        return IdleDeck.TakeNCards(random, PlayableCardsPerPlayerCount / 2);
    }

    public void TakeNCardsToHand(Random random, int n)
    {
        var maxCardsCanTakeCount = MaxHandCardsCount - HandDeck.Count;
        n = n.Clamp(maxCardsCanTakeCount);
        HandDeck = IdleDeck.TakeNCards(random, n);
    }
}

public static class PlayerExtensions
{
    public static FieldDeck ShuffleAllAndTakeHalfCards(this IEnumerable<Player> players, Random random) =>
        players
            .Select(player => player.ShuffleAllAndTakeHalfCards(random))
            .AggregateOrDefault((x, y) => x + y);

    public static void TakeNCardsToHand(this IEnumerable<Player> players, Random random, int n = Player.MaxHandCardsCount) =>
        players.ForEach(player => player.TakeNCardsToHand(random, n));

    public static Player[] GetPlayersOrder(this IEnumerable<Player> players) =>
        players.OrderByDescending(p => p.HandDeck.SpeedSum).ToArray();
}
