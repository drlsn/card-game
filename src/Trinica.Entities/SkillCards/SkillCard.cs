using Corelibs.Basic.DDD;
using Trinica.Entities.Shared;

namespace Trinica.Entities.SkillCards;

public record SkillCardId(string Value) : CardId(Value);

internal class SkillCard : Entity<SkillCardId>
{
}
