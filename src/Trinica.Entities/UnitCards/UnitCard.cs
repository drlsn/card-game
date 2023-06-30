using Corelibs.Basic.DDD;

namespace CardGame.Entities.UnitCards;

public record UnitCardId(string Value) : EntityId(Value);

internal class UnitCard : Entity<UnitCardId>
{
}
