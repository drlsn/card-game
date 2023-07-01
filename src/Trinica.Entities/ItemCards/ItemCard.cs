using Corelibs.Basic.DDD;
using Trinica.Entities.Shared;

namespace Trinica.Entities.ItemCards;

public record ItemCardId(string Value) : CardId(Value);

internal class ItemCard : Entity<ItemCardId>
{

}
