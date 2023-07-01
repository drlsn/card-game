using Trinica.Entities.Decks;
using Trinica.Entities.Gameplay.Cards;
using Trinica.Entities.Users;
using Corelibs.Basic.Collections;
using Corelibs.Basic.DDD;
using Corelibs.Basic.Maths;
using Trinica.Entities.Shared;

namespace Trinica.Entities.Gameplay;

public class Player : Entity<UserId>
{
    public const int PlayableCardsPerPlayerCount = 30;
    public const int MaxHandCardsCount = 6;
    public const int MaxBattlingCardsCount = 6;

    public DeckId DeckId { get; private set; }
    public HeroCard HeroCard { get; private set; }
    public FieldDeck IdleDeck { get; private set; }
    public FieldDeck HandDeck { get; private set; }
    public FieldDeck BattlingDeck { get; private set; }

    public FieldDeck ShuffleAllAndTakeHalfCards(Random random)
    {
        IdleDeck.ShuffleAll(random);
        return IdleDeck.TakeCards(random, PlayableCardsPerPlayerCount / 2);
    }

    public void TakeCardsToHand(Random random, int n)
    {
        var maxCardsCanTakeCount = MaxHandCardsCount - HandDeck.Count;
        n = n.Clamp(maxCardsCanTakeCount);
        HandDeck = IdleDeck.TakeCards(random, n);
    }

    public bool LayCardsToBattle(CardId[] cards)
    {
        var maxCardsCanTakeCount = MaxBattlingCardsCount - cards.Length;
        if (cards.Length > maxCardsCanTakeCount)
            return false;

        BattlingDeck += HandDeck.TakeCards(cards);

        return true;
    }
}

public static class PlayerExtensions
{
    public static FieldDeck ShuffleAllAndTakeHalfCards(this IEnumerable<Player> players, Random random) =>
        players
            .Select(player => player.ShuffleAllAndTakeHalfCards(random))
            .AggregateOrDefault((x, y) => x + y);

    public static void TakeCardsToHand(this IEnumerable<Player> players, Random random, int n = Player.MaxHandCardsCount) =>
        players.ForEach(player => player.TakeCardsToHand(random, n));

    public static Player[] GetPlayersOrder(this IEnumerable<Player> players) =>
        players.OrderByDescending(p => p.HandDeck.SpeedSum).ToArray();
}
