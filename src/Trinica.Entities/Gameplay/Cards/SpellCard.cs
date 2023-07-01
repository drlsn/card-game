using Trinica.Entities.SpellCards;

namespace Trinica.Entities.Gameplay.Cards;

public class SpellCard : ICard<SpellCardId>
{
    public SpellCardId Id { get; private set; }

}
