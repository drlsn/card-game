using Trinica.Entities.SkillCards;

namespace Trinica.Entities.Gameplay.Cards;

public class SkillCard : ICard<SkillCardId>
{
    public SkillCardId Id { get; private set; }
}
