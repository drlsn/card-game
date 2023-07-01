using Trinica.Entities.HeroCards;

namespace Trinica.Entities.Gameplay.Cards;

public class HeroCard : ICard<HeroCardId>
{
    public HeroCardId Id { get; private set; }
    public StatisticPointGroup Statistics { get; private set; }
    public SlotGroup Slots { get; private set; }
}
