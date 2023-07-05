using Trinica.Entities.HeroCards;
using Trinica.Entities.Shared;

namespace Trinica.Entities.Gameplay.Cards;

public class HeroCard : ICard, ICardWithSlots, ICardWithStats, ICombatCard
{
    public HeroCardId Id { get; private set; }
    public StatisticPointGroup Statistics { get; private set; }
    public SlotGroup Slots { get; private set; }

    public List<IEffect> Effects { get; private set; }

    CardId ICard.Id => Id;
}
