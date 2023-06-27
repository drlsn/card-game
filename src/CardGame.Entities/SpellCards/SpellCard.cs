using Corelibs.Basic.DDD;

namespace CardGame.Entities.SpellCards;

public record SpellCardId(string Value) : EntityId(Value);

public class SpellCard : Entity<SpellCardId>
{
}
