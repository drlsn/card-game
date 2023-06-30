using CardGame.Entities.ItemCards;

namespace CardGame.Entities.Gameplay.Cards;

public class ItemCard : ICard
{
    public ItemCardId Id { get; private set; }
}
