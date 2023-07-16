using Trinica.Entities.Shared;
using Trinica.Entities.UnitCards;

namespace Trinica.Entities.Gameplay.Cards;

public class UnitCard : Card, ICard, ICardWithSlots, ICardWithStats, ICombatCard, ICardWithItems
{
    public UnitCardId Id { get; private set; }
    public StatisticPointGroup Statistics { get; private set; }
    public SlotGroup Slots { get; private set; } = new();

    public List<IEffect> Effects { get; private set; } = new();

    public List<ItemCard> ItemCards => Slots.ItemCards;

    public UnitCard(
        UnitCardId id,
        StatisticPointGroup statistics) 
    {
        Id = id;
        Statistics = statistics;
    }

    public UnitCard(
        UnitCardId id,
        string name,
        Race race,
        Class @class,
        Fraction fraction,
        StatisticPointGroup statistics) : base(name, race, @class, fraction)
    {
        Id = id;
        Statistics = statistics;
    }

    CardId ICard.Id => Id;
    CardId ICombatCard.Id => Id;

    public bool DoesPowerDamage(int skillIndex)
    {
        var skillCard = Slots.SkillCards[skillIndex];
        return skillCard.DoesDamage;
    }

    public IEffect[] GetEffects(int skillIndex)
    {
        var skillCard = Slots.SkillCards[skillIndex];
        return skillCard.Effects;
    }

    public override string ToString() => Id.Value;
}
