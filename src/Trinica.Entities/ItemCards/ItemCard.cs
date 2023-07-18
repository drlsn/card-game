using Corelibs.Basic.DDD;
using Trinica.Entities.Shared;

namespace Trinica.Entities.ItemCards;

public class ItemCardId : CardId
{
    public ItemCardId(string value) : base(value)
    {
    }
}


internal class ItemCard : Entity<ItemCardId>
{

}
