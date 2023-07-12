using Trinica.Entities.Shared;
using Trinica.Entities.SkillCards;

namespace Trinica.Entities.Gameplay.Cards;

public class SkillCard : ICard
{
    public SkillCard(SkillCardId id, IEffect[] effects, int? damage = null)
    {
        Id = id;
        Effects = effects;
        Damage = damage;
    }

    public SkillCardId Id { get; private set; }
    CardId ICard.Id => Id;

    public bool DoesDamage => Damage is not null && Damage > 0;
    public int? Damage { get; init; }
    public IEffect[] Effects { get; init; }

    public override string ToString() => Id.Value;
}
