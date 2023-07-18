using Corelibs.Basic.DDD;
using Trinica.Entities.Shared;

namespace Trinica.Entities.SkillCards;

public class SkillCardId : CardId
{
    public SkillCardId(string value) : base(value)
    {
    }
}

internal class SkillCard : Entity<SkillCardId>
{
}
