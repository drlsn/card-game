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

    public bool LayCardsToBattle(CardToLay[] cards)
    {
        var maxCardsCanTake = MaxBattlingCardsCount - cards.Length;
        if (cards.Length > maxCardsCanTake)
            return false;

        var cardsIds = cards.ToCardIds();
        
        var handCards = HandDeck.TakeCards(cardsIds).GetAllCards().ToIdDict();
        var battlingCards = BattlingDeck.GetAllCards().Prepend(HeroCard).ToIdDict();

        var cardsToAdd = new List<ICard>();
        cards.ForEach(cardToLay =>
        {
            if (!handCards.TryGetValue(cardToLay.SourceCardId, out var handCard))
                return;

            if (battlingCards.TryGetValue(cardToLay.TargetCardId, out var battlingCard) &&
                battlingCard is ICardWithSlots cardWithSlots)
            {
                cardWithSlots.Slots.AddCard(handCard);
            }
            else
            {
                cardsToAdd.Add(handCard);
            }
        });

        BattlingDeck += cardsToAdd;

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

public static class CardsExtensions
{
    public static CardId[] ToCardIds(this IEnumerable<CardToLay> cards) =>
        cards.Select(c => c.SourceCardId).ToArray();

    public static IDictionary<CardId, ICard> ToIdDict(this IEnumerable<ICard> cards) =>
        cards.ToDictionary(c => c.Id);
}
