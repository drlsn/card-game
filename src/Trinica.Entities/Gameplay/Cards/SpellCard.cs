using Trinica.Entities.Shared;
using Trinica.Entities.SpellCards;

namespace Trinica.Entities.Gameplay.Cards;

public class SpellCard : ICard, ICombatCard
{
    public SpellCardId Id { get; private set; }

    public StatisticPointGroup Statistics { get; private set; }

    public List<IEffect> Effects { get; private set; }

    public SpellCard(
       SpellCardId id,
       StatisticPointGroup statistics,
       IEnumerable<IEffect> effects, 
       int? damage = null)
    {
        Id = id;
        Statistics = statistics;
        Effects = effects.ToList();
        Damage = damage;
    }

    CardId ICard.Id => Id;
    CardId ICombatCard.Id => Id;

    public bool DoesDamage => Damage is not null && Damage > 0;
    public int? Damage { get; init; }

    public bool DoesPowerDamage(int skillIndex) => DoesDamage;

    public IEffect[] GetEffects(int skillIndex) => Effects.ToArray();

    public override string ToString() => Id;
}
