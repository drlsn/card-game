using Trinica.Entities.Shared;
using Trinica.Entities.UnitCards;

namespace Trinica.Entities.Gameplay.Cards;

public class UnitCard : ICard, ICardWithSlots, ICardWithStats, ICombatCard
{
    public UnitCardId Id { get; private set; }
    public StatisticPointGroup Statistics { get; private set; }
    public SlotGroup Slots { get; private set; }

    public List<IEffect> Effects { get; private set; }

    CardId ICard.Id => Id;
}
