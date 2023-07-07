using Corelibs.Basic.Collections;
using Corelibs.Basic.DDD;
using Corelibs.Basic.Functional;
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
    public FieldDeck IdleDeck { get; private set; }

    public HeroCard HeroCard { get; private set; }
    public FieldDeck HandDeck { get; private set; } = new();
    public FieldDeck BattlingDeck { get; private set; } = new();
    public FieldDeck DeadDeck { get; private set; } = new();
    public List<DiceOutcome> FreeDiceOutcomes { get; private set; }
    public Dictionary<CardId, CardAssignment> CardAssignments { get; private set; } = new();

    public Player(
        UserId playerId,
        DeckId deckId,
        HeroCard heroCard,
        FieldDeck idleDeck) : base(playerId)
    {
        DeckId = deckId;
        HeroCard = heroCard;
        IdleDeck = idleDeck;
    }

    public FieldDeck ShuffleAllAndTakeHalfCards(Random random)
    {
        IdleDeck.ShuffleAll(random);
        return IdleDeck.TakeCards(random, IdleDeck.Count / 2);
    }

    public ICard GetBattlingCard(CardId cardId)
    {
        if (HeroCard.Id == cardId)
            return HeroCard;

        return BattlingDeck.GetCard(cardId);
    }

    public ICard[] GetBattlingCards() => BattlingDeck.GetAllCards().Prepend(HeroCard).ToArray();

    public ICard TakeCardFromHand(CardId cardId)
    {
        return HandDeck.TakeCard(cardId);
    }

    public ICard TakeCardFromBattling(CardId cardId)
    {
        if (HeroCard.Id == cardId)
        {
            var heroCard = HeroCard; HeroCard = null;
            return heroCard;
        }

        return BattlingDeck.TakeCard(cardId);
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
        HandDeck += IdleDeck.TakeCards(random, n);
    }

    public bool LayCardsToBattle(CardToLay[] cards)
    {
        if (cards.Length == 0)
            return true;

        var maxCardsCanTake = MaxBattlingCardsCount - cards.Length;
        if (cards.Length > maxCardsCanTake)
            return false;

        var cardsIds = cards.ToCardIds();
        
        var handCards = HandDeck.TakeCards(cardsIds).GetAllCards().ToIdDict();
        var battlingCards = BattlingDeck.GetAllCards().Prepend(HeroCard).ToIdDict();

        var cardsToAdd = new List<ICard>();
        foreach (var cardToLay in cards)
        {
            if (!handCards.TryGetValue(cardToLay.SourceCardId, out var handCard))
                continue;

            var hasTargetCard = cardToLay.TargetCardId is not null;
            if (hasTargetCard)
            {
                var doesTargetCardExist = battlingCards.TryGetValue(cardToLay.TargetCardId, out var battlingCard);
                var cardWithSlots = battlingCard as ICardWithSlots;
                var canTargetCardBePutToSlot = handCard is SkillCard || handCard is ItemCard;
                if (hasTargetCard &&
                    doesTargetCardExist &&
                    canTargetCardBePutToSlot &&
                    cardWithSlots is not null)
                {
                    cardWithSlots.Slots.AddCard(handCard);
                    continue;
                }
            }
            
            cardsToAdd.Add(handCard);
        }

        BattlingDeck += cardsToAdd;

        return true;
    }

    public void PlayDices(Func<Random> getRandom) =>
        PlayDices(BattlingDeck.Count + 1, getRandom);

    public void PlayDices(int n, Func<Random> getRandom)
    {
        n = n.Clamp(BattlingDeck.Count + 1);

        FreeDiceOutcomes = Enumerable.Range(0, n)
            .Select(i => Dice.Play(getRandom()))
            .ToList();

    }

    public bool AssignDiceToCard(int diceIndex, CardId cardId)
    {
        if (FreeDiceOutcomes.IsNullOrEmpty() || diceIndex >= FreeDiceOutcomes.Count)
            return false;

        var battlingCards = GetBattlingCards();
        if (!battlingCards.Contains(c => c.Id == cardId))
            return false;

        // TO DO: prevent assigning no your cards!
        CardAssignments.TryGetOrAddValue(cardId).DiceOutcome = FreeDiceOutcomes[diceIndex];
        FreeDiceOutcomes.RemoveAt(diceIndex);

        return true;
    }

    public void RemoveDiceFromCard(CardId cardId)
    {
        var card = CardAssignments.TryGetOrAddValue(cardId);
        FreeDiceOutcomes.Add(card.DiceOutcome);
        card.DiceOutcome = null;
    }

    public void ChooseCardSkill(CardId cardId, int skillIndex)
    {
        CardAssignments.TryGetOrAddValue(cardId).SkillIndex = skillIndex;
    }

    public void AssignCardTarget(CardId cardId, CardId targetCardId)
    {
        var assignment = CardAssignments.TryGetOrAddValue(cardId);
        assignment.SourceCardId = cardId;
    }

    public void RemoveCardTarget(CardId cardId, CardId targetCardId)
    {
        CardAssignments.TryGetOrAddValue(cardId).TargetCardIds.Remove(targetCardId);
    }

    public void InflictDamage(int damage, CardId cardId)
    {
        var card = GetBattlingCard(cardId);
        if (card is ICombatCard combatCard)
        {
            combatCard.Statistics.HP.ModifyClamped(-damage);
            var hp = combatCard.Statistics.HP.CalculatedValue;
            if (hp > 0)
                return;
        }

        card = TakeCardFromBattling(cardId);
        DeadDeck += card;
    }

    public bool IsCardDead(ICard card) =>
        card is HeroCard && HeroCard is null ||
        DeadDeck.Contains(card);
}

public static class PlayerExtensions
{
    public static FieldDeck ShuffleAllAndTakeHalfCards(this IEnumerable<Player> players, Random random) =>
        players
            .Select(player => player.ShuffleAllAndTakeHalfCards(random))
            .ToArray()
            .AggregateOrDefault((x, y) =>
            {
                return x + y;
            });

    public static void TakeCardsToHand(this IEnumerable<Player> players, Random random, int n = Player.MaxHandCardsCount) =>
        players.ForEach(player => player.TakeCardsToHand(random, n));

    public static Player[] GetPlayersOrderedByHeroSpeed(this IEnumerable<Player> players) =>
        players.OrderByDescending(p => p.HeroCard.Statistics.Speed.CalculatedValue).ToArray();

    public static Player OfId(this IEnumerable<Player> players, UserId id) =>
        players.First(p => p.Id == id);

    public static Player[] NotOfId(this IEnumerable<Player> players, UserId id) =>
        players.Where(p => p.Id != id).ToArray();

    public static UserId[] ToIds(this IEnumerable<Player> players) =>
        players.Select(c => c.Id).ToArray();

    public static ICard[] GetBattlingCardsBySpeed(this IEnumerable<Player> players, Random random) =>
        players
            .Select(c => c.BattlingDeck)
            .AggregateOrDefault((x, y) => x + y)
            .ThenSelect(deck => deck.SpellCards.Shuffle().Cast<ICard>().Concat(deck.UnitCards.Cast<ICardWithStats>().Concat(players.Select(p => p.HeroCard)).OrderByDescending(c => c.Statistics.Speed.CalculatedValue).Cast<ICard>()))
            .ToArray();

    public static ICard[] GetBattlingCards(this IEnumerable<Player> players) =>
        players
            .Select(c => c.BattlingDeck)
            .Aggregate((x, y) => x + y)
            .GetAllCards()
            .Concat(players.Select(p => p.HeroCard))
            .ToArray();

    public static Player GetPlayerWithCard(this IEnumerable<Player> players, CardId cardId) =>
        players
            .First(p => p.BattlingDeck
                .GetAllCards()
                .Append(p.HeroCard)
                .Contains(c => c.Id == cardId));
}

public static class CardsExtensions
{
    public static CardId[] ToCardIds(this IEnumerable<CardToLay> cards) =>
        cards.Select(c => c.SourceCardId).ToArray();

    public static IDictionary<CardId, ICard> ToIdDict(this IEnumerable<ICard> cards) =>
        cards.ToDictionary(c => c.Id);
}
