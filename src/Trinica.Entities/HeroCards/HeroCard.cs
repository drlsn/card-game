using Corelibs.Basic.DDD;
using Trinica.Entities.Shared;

namespace Trinica.Entities.HeroCards;

public record HeroCardId(string Value) : CardId(Value);

internal class HeroCard : Entity<HeroCardId>
{
}
