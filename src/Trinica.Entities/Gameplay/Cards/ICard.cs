using Corelibs.Basic.Collections;
using Trinica.Entities.Shared;

namespace Trinica.Entities.Gameplay.Cards;

public interface ICard
{
    CardId Id { get; }
    string Name { get; }
    Race Race { get; }
    Class Class { get; }
    Fraction Fraction { get; }
}

public static class CardExtensions
{
    public static bool ContainsOfId(this IEnumerable<ICard> cards, CardId id) =>
        cards.Contains(card => card.Id == id);
}
