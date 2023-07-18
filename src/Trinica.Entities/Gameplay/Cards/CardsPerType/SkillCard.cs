using Trinica.Entities.Shared;
using Trinica.Entities.SkillCards;

namespace Trinica.Entities.Gameplay.Cards;

public class SkillCard : Card, ICard
{
    public SkillCard(
        SkillCardId id, 
        string name,
        Race race,
        Class @class,
        Fraction fraction,
        IEffect[] effects,
        int? damage = null) : base(name, race, @class, fraction)
    {
        Id = id;
        Effects = effects;
        Damage = damage;
    }

    public SkillCard(SkillCardId id, IEffect[] effects, int? damage = null)
    {
        Id = id;
        Effects = effects;
        Damage = damage;
    }

    public SkillCardId Id { get; private set; }
    CardId ICard.Id => new CardId(Id.Value);

    public bool DoesDamage => Damage is not null && Damage > 0;
    public int? Damage { get; init; }
    public IEffect[] Effects { get; init; }

    public override string ToString() => Id.Value;
}
