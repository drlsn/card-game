using Trinica.Entities.Gameplay.Cards;

namespace Trinica.Entities.Gameplay;

public interface ICardWithItems
{
    List<ItemCard> ItemCards { get; }
}
