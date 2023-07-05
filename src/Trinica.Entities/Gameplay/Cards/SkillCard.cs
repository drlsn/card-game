using Trinica.Entities.Shared;
using Trinica.Entities.SkillCards;

namespace Trinica.Entities.Gameplay.Cards;

public class SkillCard : ICard
{
    public SkillCardId Id { get; private set; }
    CardId ICard.Id => Id;

    public bool DoesDamage { get; init; }
    public IEffect[] Effects { get; init; }
}
