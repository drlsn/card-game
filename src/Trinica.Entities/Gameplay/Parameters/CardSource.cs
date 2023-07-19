namespace Trinica.Entities.Gameplay;

public record CardSource(string Value)
{
    public static readonly CardSource CommonPool = new CardSource("common-pool");
    public static readonly CardSource Own = new CardSource("own");
}

public static class CardSourceExtensions
{
    public static CardToTake ToCardToTake(this string cardSource) =>
        new CardToTake(new(cardSource));

    public static CardToTake[] ToCardsToTake(this string[] cardSources) =>
        cardSources.Select(c => c.ToCardToTake()).ToArray();
}
