using Corelibs.Basic.DDD;

namespace CardGame.Entities.HeroCards;

public record HeroCardId(string Value) : EntityId(Value);

internal class HeroCard : Entity<HeroCardId>
{
}
