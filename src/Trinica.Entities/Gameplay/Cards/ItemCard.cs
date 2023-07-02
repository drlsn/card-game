using Trinica.Entities.ItemCards;
using Trinica.Entities.Shared;

namespace Trinica.Entities.Gameplay.Cards;

public class ItemCard : ICard
{
    public ItemCardId Id { get; private set; }
    CardId ICard.Id => Id;
}
