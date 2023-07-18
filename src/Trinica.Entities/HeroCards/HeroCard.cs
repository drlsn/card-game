using Corelibs.Basic.DDD;
using Trinica.Entities.Shared;

namespace Trinica.Entities.HeroCards;

public class HeroCardId : CardId
{
    public HeroCardId(string value) : base(value)
    {
    }
}

internal class HeroCard : Entity<HeroCardId>
{
}
