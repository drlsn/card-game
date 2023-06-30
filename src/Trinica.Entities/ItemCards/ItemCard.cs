using Corelibs.Basic.DDD;

namespace CardGame.Entities.ItemCards;

public record ItemCardId(string Value) : EntityId(Value);

internal class ItemCard : Entity<ItemCardId>
{

}
