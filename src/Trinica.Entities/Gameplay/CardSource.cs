namespace Trinica.Entities.Gameplay;

public record CardSource(string Value)
{
    public static readonly CardSource CommonPool = new CardSource("common-pool");
    public static readonly CardSource Own = new CardSource("own");
}

public record CardToTake(CardSource Source);
