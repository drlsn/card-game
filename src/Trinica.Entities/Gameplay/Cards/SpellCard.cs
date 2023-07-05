using Trinica.Entities.Shared;
using Trinica.Entities.SpellCards;

namespace Trinica.Entities.Gameplay.Cards;

public class SpellCard : ICard, ICombatCard
{
    public SpellCardId Id { get; private set; }

    public StatisticPointGroup Statistics { get; private set; }

    public List<IEffect> Effects { get; private set; }

    CardId ICard.Id => Id;
    CardId ICombatCard.Id => Id;

    public bool DoesDamage { get; init; }

    public bool DoesPowerDamage(int skillIndex) => DoesDamage;

    public IEffect[] GetEffects(int skillIndex) => Effects.ToArray();
}
