using Corelibs.Basic.DDD;
using Trinica.Entities.Shared;

namespace Trinica.Entities.UnitCards;

public record UnitCardId(string Value) : CardId(Value);

internal class UnitCard : Entity<UnitCardId>
{
}
