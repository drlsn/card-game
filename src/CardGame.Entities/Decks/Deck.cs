namespace CardGame.Entities.Decks;

public interface ICardId
{

}

public record HeroCardId(string Value) : ICardId;
public record UnitCardId(string Value) : ICardId;
public record SkillCardId(string Value) : ICardId;
public record ItemCardId(string Value) : ICardId;
public record SpellCardId(string Value) : ICardId;

public record DeckId(string Value);
public record Deck(
    DeckId Id,
    HeroCardId HeroCardId,
    List<ICardId> CardIds)
{
    public const int MaxCardCount = 30;
    public bool IsValid() => CardIds.Count == MaxCardCount;
}
