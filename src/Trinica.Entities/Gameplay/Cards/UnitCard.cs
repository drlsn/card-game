using Trinica.Entities.Shared;
using Trinica.Entities.UnitCards;

namespace Trinica.Entities.Gameplay.Cards;

public class UnitCard : ICard, ICardWithSlots, ICardWithStats, ICombatCard, ICardWithItems
{
    public UnitCardId Id { get; private set; }
    public StatisticPointGroup Statistics { get; private set; }
    public SlotGroup Slots { get; private set; }

    public List<IEffect> Effects { get; private set; }

    public List<ItemCard> ItemCards => Slots.ItemCards;

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
}
