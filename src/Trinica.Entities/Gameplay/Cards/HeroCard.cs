using CardGame.Entities.HeroCards;

namespace CardGame.Entities.Gameplay.Cards;

public class HeroCard : ICard
{
    public HeroCardId Id { get; private set; }
    public StatisticPointGroup Statistics { get; private set; }
}
