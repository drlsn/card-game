using Corelibs.Basic.DDD;
using Trinica.Entities.Shared;

namespace Trinica.Entities.UnitCards;

public class UnitCardId : CardId
{
    public UnitCardId(string value) : base(value)
    {
    }
}

internal class UnitCard : Entity<UnitCardId>
{
}
