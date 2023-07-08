namespace Trinica.Entities.Gameplay;

public record Element(string Value)
{
    public static readonly Element Fire = new Element("fire");
    public static readonly Element Ice = new Element("ice");
    public static readonly Element Storm = new Element("storm");
    public static readonly Element Earth = new Element("earth");
}
