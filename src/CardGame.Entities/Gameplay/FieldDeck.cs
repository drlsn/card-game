using CardGame.Entities.Gameplay.Cards;
using Corelibs.Basic.Collections;

namespace CardGame.Entities.Gameplay;

public class FieldDeck
{
    public const int CardsCount = 30;
    public const int CardsHalfCount = CardsCount / 2;
    public const int CardTypesCount = 4;

    public List<UnitCard> UnitCards { get; private set; }
    public List<SkillCard> SkillCards { get; private set; }
    public List<ItemCard> ItemCards { get; private set; }
    public List<SpellCard> SpellCards { get; private set; }

    public FieldDeck(
        List<UnitCard> unitCards, 
        List<SkillCard> skillCards, 
        List<ItemCard> itemCards, 
        List<SpellCard> spellCards)
    {
        UnitCards = unitCards;
        SkillCards = skillCards;
        ItemCards = itemCards;
        SpellCards = spellCards;
    }

    public FieldDeck ShuffleAllAndTakeHalfCards(Random random) =>
        TakeAndShuffleNCards(random, CardsCount / 2);
        
    public FieldDeck TakeAndShuffleNCards(Random random, int n)
    {
        var unitCards = UnitCards.Shuffle(random).ToRemoveOnlyList();
        var skillCards = SkillCards.Shuffle(random).ToRemoveOnlyList();
        var itemCards = ItemCards.Shuffle(random).ToRemoveOnlyList();
        var spellCards = SpellCards.Shuffle(random).ToRemoveOnlyList();

        int cardsHalfCount = n / 2;
        var cardsCluster = new IRemoveOnlyList<object>[] { unitCards, skillCards, itemCards, spellCards };
        var cardsClusterSelected =
            Enumerable.Range(0, CardTypesCount)
                .Select(i => new List<object>(cardsHalfCount))
            .ToArray();

        for (var i = 0; i < cardsHalfCount; i++)
        {
            var cardGroupIndex = random.Next(CardTypesCount);
            var cardGroup = cardsCluster[cardGroupIndex];
            var cardIndex = random.Next(cardGroup.Count);

            var cardSelected = cardGroup[cardIndex];
            cardsClusterSelected[cardIndex].Add(cardSelected);
            cardGroup.RemoveAt(cardIndex);
        }

        UnitCards = unitCards.CastToList<UnitCard>();
        SkillCards = skillCards.CastToList<SkillCard>();
        ItemCards = itemCards.CastToList<ItemCard>();
        SpellCards = spellCards.CastToList<SpellCard>();

        return new FieldDeck(
            cardsClusterSelected[0].CastToList<UnitCard>(),
            cardsClusterSelected[1].CastToList<SkillCard>(),
            cardsClusterSelected[2].CastToList<ItemCard>(),
            cardsClusterSelected[3].CastToList<SpellCard>());
    }

    public static FieldDeck operator +(FieldDeck left, FieldDeck right)
    {
        return new(
            left.UnitCards.Concat(right.UnitCards).ToList(),
            left.SkillCards.Concat(right.SkillCards).ToList(),
            left.ItemCards.Concat(right.ItemCards).ToList(),
            left.SpellCards.Concat(right.SpellCards).ToList());
    }
}
