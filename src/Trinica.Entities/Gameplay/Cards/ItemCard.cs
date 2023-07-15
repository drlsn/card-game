using Trinica.Entities.ItemCards;
using Trinica.Entities.Shared;

namespace Trinica.Entities.Gameplay.Cards;

public class ItemCard : Card, ICard
{
    public ItemCard(ItemCardId id, StatisticPointGroup statistics)
    {
        Id = id;
        Statistics = statistics;
    }

    public ItemCard(
        ItemCardId id,
        string name,
        Race race,
        Class @class,
        Fraction fraction, 
        StatisticPointGroup statistics) : base(name, race, @class, fraction)
    {
        Id = id;
        Statistics = statistics;
    }

    public ItemCardId Id { get; private set; }
    CardId ICard.Id => Id;
    public StatisticPointGroup Statistics { get; private set; }
    public bool IsActive { get; private set; } = true;

    public override string ToString() => Id.Value;
}
