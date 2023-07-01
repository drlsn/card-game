using Corelibs.Basic.DDD;
using Trinica.Entities.Shared;

namespace Trinica.Entities.SpellCards;

public record SpellCardId(string Value) : CardId(Value);

public class SpellCard : Entity<SpellCardId>
{
}
