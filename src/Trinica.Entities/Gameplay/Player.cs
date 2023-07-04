using Corelibs.Basic.Collections;
using Corelibs.Basic.DDD;
using Corelibs.Basic.Maths;
using Trinica.Entities.Decks;
using Trinica.Entities.Gameplay.Cards;
using Trinica.Entities.Shared;
using Trinica.Entities.Users;

namespace Trinica.Entities.Gameplay;

public class Player : Entity<UserId>
{
    public const int TotalCardsPerPlayerCount = 30;
    public const int MaxHandCardsCount = 6;
    public const int MaxBattlingCardsCount = 6;

    public DeckId DeckId { get; private set; }
    public HeroCard HeroCard { get; private set; }
    public FieldDeck IdleDeck { get; private set; }
    public FieldDeck HandDeck { get; private set; }
    public FieldDeck BattlingDeck { get; private set; }
    public DiceOutcomePerCard[] DiceOutcomesPerCard { get; private set; }

    public FieldDeck ShuffleAllAndTakeHalfCards(Random random)
    {
        IdleDeck.ShuffleAll(random);
        return IdleDeck.TakeCards(random, TotalCardsPerPlayerCount / 2);
    }

    public ICard GetBattlingCard(CardId cardId)
    {
        if (HeroCard.Id == cardId)
            return HeroCard;

        return BattlingDeck.GetCard(cardId);
    }

    public ICard TakeCardFromHand(CardId cardId)
    {
        return HandDeck.TakeCard(cardId);
    }

    public void AddCardToHand(ICard card)
    {
        HandDeck += card;
    }

    public void TakeCardToHand(Random random) =>
        TakeCardsToHand(random, 1);

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

    public void PlayDices(int n, Func<Random> getRandom)
    {
        n = n.Clamp(BattlingDeck.Count);

        DiceOutcomesPerCard = Enumerable.Range(0, n)
            .Select(i => new DiceOutcomePerCard(Dice.Play(getRandom())))
            .ToArray();
    }

    public void AssignDicesToCards(DiceOutcomeIndexPerCard[] assigns)
    {
        DiceOutcomesPerCard = assigns
            .Select(a => new DiceOutcomePerCard(DiceOutcomesPerCard[a.DiceIndex].Outcome, a.CardId))
            .ToArray();
    }

    public void AssignCardsTargets(CardTarget[] targets)
    {
        DiceOutcomesPerCard = targets
            .Select(target => 
            {
                var assign = DiceOutcomesPerCard.First(c => c.SourceCardId == target.SourceCardId);
                return new DiceOutcomePerCard(assign.Outcome, assign.SourceCardId, target.TargetCardId);
            })
            .ToArray();
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
        players.OrderByDescending(p => p.HeroCard.Statistics.Speed).ToArray();

    public static Player OfId(this IEnumerable<Player> players, UserId id) =>
        players.First(p => p.Id == id);

    public static UserId[] ToIds(this IEnumerable<Player> players) =>
        players.Select(c => c.Id).ToArray();
}

public static class CardsExtensions
{
    public static CardId[] ToCardIds(this IEnumerable<CardToLay> cards) =>
        cards.Select(c => c.SourceCardId).ToArray();

    public static IDictionary<CardId, ICard> ToIdDict(this IEnumerable<ICard> cards) =>
        cards.ToDictionary(c => c.Id);
}
