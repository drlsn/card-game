using Corelibs.Basic.DDD;
using Trinica.Entities.Shared;

namespace Trinica.Entities.SpellCards;

public class SpellCardId : CardId
{
    public SpellCardId(string value) : base(value)
    {
    }
}


public class SpellCard : Entity<SpellCardId>
{
}
