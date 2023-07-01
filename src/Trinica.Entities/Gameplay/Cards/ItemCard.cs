using Trinica.Entities.HeroCards;
using Trinica.Entities.ItemCards;

namespace Trinica.Entities.Gameplay.Cards;

public class ItemCard : ICard<ItemCardId>
{
    public ItemCardId Id { get; private set; }
}
