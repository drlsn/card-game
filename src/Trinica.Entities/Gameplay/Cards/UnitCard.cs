using CardGame.Entities.UnitCards;

namespace CardGame.Entities.Gameplay.Cards;

public class UnitCard : ICard
{
    public UnitCardId Id { get; private set; }
    public StatisticPointGroup Statistics { get; private set; }
}
