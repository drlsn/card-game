namespace Trinica.Entities.Shared;

public record Class(string Name)
{
    public static readonly Class Assasin = new("Assasin");
    public static readonly Class Guardian = new("Guardian");
    public static readonly Class Wizard = new("Wizard");
    public static readonly Class Destroyer = new("Destroyer");
    public static readonly Class Support = new("Support");
}
