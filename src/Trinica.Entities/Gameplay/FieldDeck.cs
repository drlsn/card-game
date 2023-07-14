using Corelibs.Basic.Collections;
using System.Linq;
using Trinica.Entities.Gameplay.Cards;
using Trinica.Entities.Shared;

namespace Trinica.Entities.Gameplay;

public class FieldDeck
{
    public const int CardTypesCount = 4;

    public List<UnitCard> UnitCards { get; private set; } = new();
    public List<SkillCard> SkillCards { get; private set; } = new();
    public List<ItemCard> ItemCards { get; private set; } = new();
    public List<SpellCard> SpellCards { get; private set; } = new();

    public int Count =>
        UnitCards.Count + 
        SkillCards.Count +
        ItemCards.Count +
        SpellCards.Count;

    public int SpeedSum =>
        UnitCards.Select(c => c.Statistics.Speed.CalculatedValue).Sum();

    public FieldDeck(IEnumerable<ICard> cards)
    {
        this.Assign(cards);
    }

    public FieldDeck(
        List<UnitCard> unitCards = null,
        List<SkillCard> skillCards = null, 
        List<ItemCard> itemCards = null, 
        List<SpellCard> spellCards = null)
    {
        UnitCards = unitCards ?? new();
        SkillCards = skillCards ?? new();
        ItemCards = itemCards ?? new();
        SpellCards = spellCards ?? new();
    }


    public void Clear()
    {
        UnitCards.Clear();
        SkillCards.Clear();
        ItemCards.Clear();
        SpellCards.Clear();
    }

    public void ShuffleAll(Random random)
    {
        var deck = ShuffleAll(this, random);

        UnitCards = deck.UnitCards;
        SkillCards = deck.SkillCards;
        ItemCards = deck.ItemCards;
        SpellCards = deck.SpellCards;
    }

    public static FieldDeck ShuffleAll(FieldDeck deck, Random random)
    {
        return new(
            deck.UnitCards.Shuffle(random).ToList(),
            deck.SkillCards.Shuffle(random).ToList(),
            deck.ItemCards.Shuffle(random).ToList(),
            deck.SpellCards.Shuffle(random).ToList());
    }

    public bool Contains(ICard card) => Contains(card.Id);
    public bool Contains(CardId cardId) => GetAllCards().FirstOrDefault(c => c.Id == cardId) is not null;

    public ICard GetCard(CardId cardId) =>
        GetAllCards().First(c => c.Id == cardId);

    public ICard TakeCard(CardId cardId) =>
        TakeCards(new[] { cardId }).GetAllCards().First();

    public FieldDeck TakeCards(CardId[] cardsToTake)
    {
        var deckToTake = new FieldDeck(
            UnitCards.Where(c => cardsToTake.Contains(c.Id)).ToList(),
            SkillCards.Where(c => cardsToTake.Contains(c.Id)).ToList(),
            ItemCards.Where(c => cardsToTake.Contains(c.Id)).ToList(),
            SpellCards.Where(c => cardsToTake.Contains(c.Id)).ToList());

        var leftDeck = this - deckToTake;

        UnitCards = leftDeck.UnitCards;
        SkillCards = leftDeck.SkillCards;
        ItemCards = leftDeck.ItemCards;
        SpellCards = leftDeck.SpellCards;

        return deckToTake;
    }

    public ICard TakeCard(Random random) =>
        TakeCards(random, 1).GetAllCards().First();

    public FieldDeck TakeCards(Random random, int n)
    {
        var cards = GetAllCards();
        var cardsShuffled = cards.Shuffle(random).ToRemoveOnlyList();
        var taken = cardsShuffled.Take(n);
        Clear();
        this.Assign(cardsShuffled);
        return new(taken);
    }

    public static FieldDeck operator +(FieldDeck left, FieldDeck right)
    {
        return new(
            left.UnitCards.Concat(right.UnitCards).ToList(),
            left.SkillCards.Concat(right.SkillCards).ToList(),
            left.ItemCards.Concat(right.ItemCards).ToList(),
            left.SpellCards.Concat(right.SpellCards).ToList());
    }

    public static FieldDeck operator +(FieldDeck left, ICard card)
    {
        var array = new[] { card };
        return left + array;
    }

    public static FieldDeck operator +(FieldDeck left, IEnumerable<ICard> right)
    {
        return new(
            left.UnitCards.Concat(right.OfType<UnitCard>()).ToList(),
            left.SkillCards.Concat(right.OfType<SkillCard>()).ToList(),
            left.ItemCards.Concat(right.OfType<ItemCard>()).ToList(),
            left.SpellCards.Concat(right.OfType<SpellCard>()).ToList());
    }

    public static FieldDeck operator -(FieldDeck left, FieldDeck right)
    {
        return new(
            left.UnitCards.Except(right.UnitCards).ToList(),
            left.SkillCards.Except(right.SkillCards).ToList(),
            left.ItemCards.Except(right.ItemCards).ToList(),
            left.SpellCards.Except(right.SpellCards).ToList());
    }

    public IEnumerable<ICard> GetAllCards() =>
        EnumerableExtensions.Concat<ICard>(
            UnitCards, SkillCards, ItemCards, SpellCards);
}

public static class FieldDeckExtensions
{
    public static void Assign(this FieldDeck deck, IEnumerable<ICard> cards)
    {
        deck.UnitCards.AddRange(cards.OfType<UnitCard>().ToList());
        deck.SkillCards.AddRange(cards.OfType<SkillCard>().ToList());
        deck.ItemCards.AddRange(cards.OfType<ItemCard>().ToList());
        deck.SpellCards.AddRange(cards.OfType<SpellCard>().ToList());
    }
}
