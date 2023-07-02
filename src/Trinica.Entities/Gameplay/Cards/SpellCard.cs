using Trinica.Entities.Shared;
using Trinica.Entities.SpellCards;

namespace Trinica.Entities.Gameplay.Cards;

public class SpellCard : ICard
{
    public SpellCardId Id { get; private set; }
    CardId ICard.Id => Id;
}
