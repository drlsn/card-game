using Trinica.Entities.Shared;
using Trinica.Entities.UnitCards;

namespace Trinica.Entities.Gameplay.Cards;

public class UnitCard : ICard
{
    public UnitCardId Id { get; private set; }
    public StatisticPointGroup Statistics { get; private set; }
    public SlotGroup Slots { get; private set; }
    CardId ICard.Id => Id;
}
