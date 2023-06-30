using CardGame.Entities.ItemCards;
using Corelibs.Basic.DDD;

namespace CardGame.Entities.SkillCards;

public record SkillCardId(string Value) : EntityId(Value);

internal class SkillCard : Entity<SkillCardId>
{
}
