using Corelibs.Basic.Collections;
using Corelibs.Basic.DDD;
using Corelibs.Basic.Maths;
using Trinica.Entities.Decks;
using Trinica.Entities.Gameplay.Cards;
using Trinica.Entities.Gameplay.Events;
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
    public List<DiceOutcome> DiceOutcomesToAssign { get; private set; }
    public Dictionary<CardId, CardAssignment> CardAssignments { get; private set; } = new();

    public Player(
        UserId id,
        DeckId deckId,
        HeroCard heroCard,
        FieldDeck idleDeck) : base(id)
    {
        DeckId = deckId;
        HeroCard = heroCard;
        IdleDeck = idleDeck;
    }

    public Player(
        UserId playerId,
        uint version,
        DeckId deckId,
        HeroCard heroCard,
        FieldDeck idleDeck,
        FieldDeck handDeck,
        FieldDeck battlingDeck,
        FieldDeck deadDeck,
        List<DiceOutcome> diceOutcomesToAssign,
        Dictionary<CardId, CardAssignment> cardAssignments) : base(playerId, version)
    {
        DeckId = deckId;
        HeroCard = heroCard;
        IdleDeck = idleDeck;
        HandDeck = handDeck;
        BattlingDeck = battlingDeck;
        DeadDeck = deadDeck;
        DiceOutcomesToAssign = diceOutcomesToAssign;
        CardAssignments = cardAssignments;
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

    public ICard[] GetBattlingCards() => BattlingDeck.GetCards().Prepend(HeroCard).ToArray();

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

    public bool CanTakeCardToHand(out int max, int commonPoolCount)
    {
        max = MaxHandCardsCount - HandDeck.Count;
        if (max > 0 && commonPoolCount > 0)
            return true;

        max = max.Clamp(IdleDeck.Count);
        return max != 0;
    }

    public void TakeCardToHand(Random random) =>
        TakeCardsToHand(random, 1);

    public void TakeCardsToHand(Random random, int n)
    {
        var maxCardsCanTakeCount = MaxHandCardsCount - HandDeck.Count;
        n = n.Clamp(maxCardsCanTakeCount);
        HandDeck += IdleDeck.TakeCards(random, n);
    }

    public bool CanLayCardDownToTarget(CardAndOwner targetCard)
    {
        var handCards = HandDeck.GetCards();
        if (targetCard is not null)
        {
            if (targetCard.PlayerId != Id)
                return false;

            if (targetCard is not ICardWithSlots targetCardWithSlots)
                return false;

            if (!targetCardWithSlots.Slots.CanAddCard())
                return false;

            return handCards.Any(card => card is SkillCard || card is ItemCard);
        }

        return handCards.Any(card => card is ICombatCard);
    }

    public bool CanLayCardDown()
    {
        if (HandDeck.Count == 0)
            return false;

        var handCards = HandDeck.GetCards();
        // Has Free Battling Spot and Combat Card in Hand
        if (BattlingDeck.Count < MaxBattlingCardsCount &&
            HandDeck.Count > 0 &&
            handCards.Any(card => card is ICombatCard))
            return true;

        // Has Some or No Battling Spots, Battling Cards with Slots Free and Skills or Items Cards in Hand
        var battlingCards = BattlingDeck.GetCards();
        var battlingCardsWithSlots = battlingCards.OfType<ICardWithSlots>().ToArray();
        var haveAnyBattlingCardWithFreeSlot = battlingCardsWithSlots.Any(card => card.Slots.CanAddCard());
        if (haveAnyBattlingCardWithFreeSlot &&
            handCards.Any(card => card is ItemCard || card is SkillCard))
            return true;

        return false;
    }

    public bool LayCardToBattle(CardToLay card) =>
        LayCardsToBattle(new[] { card });

    public bool LayCardsToBattle(CardToLay[] cards)
    {
        if (cards.Length == 0)
            return true;

        var maxCardsCanTake = MaxBattlingCardsCount - cards.Length;
        if (cards.Length > maxCardsCanTake)
            return false;

        var cardsIds = cards.ToCardIds();
        
        var handCards = HandDeck.GetCards(cardsIds).ToIdDict();
        var battlingCards = BattlingDeck.GetCards().Prepend(HeroCard).ToIdDict();

        var cardsToAdd = new List<ICard>();
        var cardsToTake = new List<ICard>();
        foreach (var cardToLay in cards)
        {
            if (!handCards.TryGetValue(cardToLay.SourceCardId.Value, out var handCard))
                continue;

            var canTargetCardBePutToSlot = handCard is SkillCard || handCard is ItemCard;
            var hasTargetCard = cardToLay.TargetCardId is not null && !cardToLay.TargetCardId.Value.IsNullOrEmpty();
            if (hasTargetCard && canTargetCardBePutToSlot)
            {
                var doesTargetCardExist = battlingCards.TryGetValue(cardToLay.TargetCardId!.Value, out var battlingCard);
                var cardWithSlots = battlingCard as ICardWithSlots;
                if (hasTargetCard &&
                    doesTargetCardExist &&
                    canTargetCardBePutToSlot &&
                    cardWithSlots is not null)
                {
                    cardWithSlots.Slots.AddCard(handCard);
                    cardsToTake.Add(handCard);
                    continue;
                }
            }
            else
            if (!canTargetCardBePutToSlot)
            {
                cardsToAdd.Add(handCard);
                cardsToTake.Add(handCard);
                continue;
            }

            //return false;
        }

        HandDeck.TakeCards(cardsToTake.Select(c => c.Id).ToArray());
        BattlingDeck += cardsToAdd;

        return true;
    }

    public void PlayDices(Func<Random> getRandom) =>
        PlayDices(BattlingDeck.Count + 1, getRandom);

    public void PlayDices(int n, Func<Random> getRandom)
    {
        n = n.Clamp(BattlingDeck.Count + 1);

        DiceOutcomesToAssign = Enumerable.Range(0, n)
            .Select(i => Dice.Play(getRandom()))
            .ToList();

        GetBattlingCards().ForEach(
            card => CardAssignments.TryAdd(card.Id, new() { SourceCardId = card.Id }));
    }

    public bool AssignDiceToCard(int diceIndex, CardId cardId)
    {
        if (DiceOutcomesToAssign.IsNullOrEmpty() || diceIndex >= DiceOutcomesToAssign.Count)
            return false;

        var battlingCards = GetBattlingCards();
        var card = battlingCards.FirstOrDefault(c => c.Id.Value == cardId.Value);
        if (card is null)
            return false;

        var diceOutcome = DiceOutcomesToAssign[diceIndex];
        if (diceOutcome.IsElement())
        {
            if (card is UnitCard || card is HeroCard)
                return false;

            if (card is not ICardWithElements cardWithElements)
                return false;
            
            var element = diceOutcome.ToElement();
            if (!cardWithElements.RequiredElements.Contains(element))
                return false;
        }
        else
        {

        }

        if (!CardAssignments.TryGetValue(cardId, out var assignment))
            return false;

        assignment.DiceOutcome = DiceOutcomesToAssign[diceIndex];
        DiceOutcomesToAssign.RemoveAt(diceIndex);

        return true;
    }

    public void RemoveDiceFromCard(CardId cardId)
    {
        var card = CardAssignments.TryGetOrAddValue(cardId);
        DiceOutcomesToAssign.Add(card.DiceOutcome);
        card.DiceOutcome = null;
    }

    public void ChooseCardSkill(CardId cardId, int skillIndex)
    {
        CardAssignments.TryGetOrAddValue(cardId).SkillIndex = skillIndex;
    }

    public bool AssignCardTarget(CardId cardId, CardId targetCardId)
    {
        var cards = GetBattlingCards();
        if (!cards.ContainsOfId(cardId))
            return false;

        // TO DO: check if target card can be really the target always depending on card
        if (!CardAssignments.TryGetValue(cardId, out var assignment))
            return false;
        
        assignment.TargetCardIds.Add(targetCardId);

        return true;
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

        KillCard(cardId);
    }

    public bool KillCard(CardId cardId)
    {
        var card = TakeCardFromBattling(cardId);
        if (card is null || card.Id != cardId)
            return false;

        DeadDeck += card;

        return true;
    }

    public bool KillCard(ICard card)
    {
        var battlingCard = TakeCardFromBattling(card.Id);
        if (battlingCard is null || battlingCard.Id != card.Id)
            return false;

        DeadDeck += card;

        return true;
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

    public static ICard[] GetBattlingCardsBySpeed(this IEnumerable<Player> players, Random random)
    {
        var battlingDecks = players.Select(c => c.BattlingDeck);
        var battlingDeck = battlingDecks.AggregateOrDefault((x, y) => x + y);

        var heroCards = players.Select(p => p.HeroCard);
        var unitCards = battlingDeck.UnitCards;
        var spellCards = battlingDeck.SpellCards.Shuffle(random);

        var unitAndHeroCards = heroCards.Concat<ICardWithStats>(unitCards);
        var unitAndHeroCardsOrdered = unitAndHeroCards.OrderByDescending(c => c.Statistics.Speed.CalculatedValue).Cast<ICard>().ToArray();
        var allCardsOrdered = spellCards.Concat(unitAndHeroCardsOrdered).ToArray();

        return allCardsOrdered;
    }

    public static ICard[] GetBattlingCards(this IEnumerable<Player> players) =>
        players
            .Select(c => c.BattlingDeck)
            .Aggregate((x, y) => x + y)
            .GetCards()
            .Concat(players.Select(p => p.HeroCard))
            .ToArray();

    public static Player GetPlayerWithCard(this IEnumerable<Player> players, CardId cardId) =>
        players
            .First(p => p.BattlingDeck
                .GetCards()
                .Append(p.HeroCard)
                .Contains(c => c.Id == cardId));
}

public static class CardsExtensions
{
    public static CardId[] ToCardIds(this IEnumerable<CardToLay> cards) =>
        cards.Select(c => c.SourceCardId).ToArray();

    public static IDictionary<string, ICard> ToIdDict(this IEnumerable<ICard> cards) =>
        cards.ToDictionary(c => c.Id.Value);
}
