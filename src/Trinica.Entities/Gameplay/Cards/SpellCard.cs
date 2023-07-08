using Trinica.Entities.Shared;
using Trinica.Entities.SpellCards;

namespace Trinica.Entities.Gameplay.Cards;

public class SpellCard : ICard, ICombatCard, ICardWithElements
{
    public SpellCardId Id { get; private set; }

    public StatisticPointGroup Statistics { get; private set; } = new();

    public List<IEffect> Effects { get; private set; }

    public SpellCard(
       SpellCardId id,
       IEnumerable<IEffect> effects, 
       IEnumerable<Element> requiredElements,
       int damage = 0)
    {
        Id = id;
        Effects = effects.ToList();
        RequiredElements = requiredElements.ToArray();
        Damage = damage;
    }

    public Element[] RequiredElements { get; }

    CardId ICard.Id => Id;
    CardId ICombatCard.Id => Id;

    public bool DoesDamage => Damage > 0;
    public int Damage { get; }

    public bool DoesPowerDamage(int skillIndex) => DoesDamage;

    public IEffect[] GetEffects(int skillIndex) => Effects.ToArray();

    public override string ToString() => Id;
}
