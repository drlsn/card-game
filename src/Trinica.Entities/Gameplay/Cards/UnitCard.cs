using Trinica.Entities.UnitCards;

namespace Trinica.Entities.Gameplay.Cards;

public class UnitCard : ICard
{
    public UnitCardId Id { get; private set; }
    public StatisticPointGroup Statistics { get; private set; }
}
