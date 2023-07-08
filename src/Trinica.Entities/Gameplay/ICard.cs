using Corelibs.Basic.Collections;
using Trinica.Entities.Shared;

namespace Trinica.Entities.Gameplay;

public interface ICard
{
    CardId Id { get; }
}

public static class CardExtensions
{
    public static bool ContainsOfId(this IEnumerable<ICard> cards, CardId id) =>
        cards.Contains(card => card.Id == id);   
}
