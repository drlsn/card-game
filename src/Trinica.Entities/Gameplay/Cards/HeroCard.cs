using Trinica.Entities.HeroCards;

namespace Trinica.Entities.Gameplay.Cards;

public class HeroCard : ICard
{
    public HeroCardId Id { get; private set; }
    public StatisticPointGroup Statistics { get; private set; }
}
