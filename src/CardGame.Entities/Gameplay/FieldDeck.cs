﻿using CardGame.Entities.Gameplay.Cards;
using Corelibs.Basic.Collections;

namespace CardGame.Entities.Gameplay;

public class FieldDeck
{
    public const int CardTypesCount = 4;

    public List<UnitCard> UnitCards { get; private set; }
    public List<SkillCard> SkillCards { get; private set; }
    public List<ItemCard> ItemCards { get; private set; }
    public List<SpellCard> SpellCards { get; private set; }

    public int Count =>
        UnitCards.Count + 
        SkillCards.Count +
        ItemCards.Count +
        SpellCards.Count;

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

    public FieldDeck TakeNCards(Random random, int n)
    {
        var unitCards = UnitCards.ToRemoveOnlyList();
        var skillCards = SkillCards.ToRemoveOnlyList();
        var itemCards = ItemCards.ToRemoveOnlyList();
        var spellCards = SpellCards.ToRemoveOnlyList();

        var cardsCluster = new IRemoveOnlyList<object>[] { unitCards, skillCards, itemCards, spellCards };
        var cardsClusterSelected =
            Enumerable.Range(0, CardTypesCount)
                .Select(i => new List<object>(n))
            .ToArray();

        for (var i = 0; i < n; i++)
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

    public FieldDeck TakeAndShuffleNCards(Random random, int n)
    {
        var unitCards = UnitCards.ToRemoveOnlyList();
        var skillCards = SkillCards.ToRemoveOnlyList();
        var itemCards = ItemCards.ToRemoveOnlyList();
        var spellCards = SpellCards.ToRemoveOnlyList();

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