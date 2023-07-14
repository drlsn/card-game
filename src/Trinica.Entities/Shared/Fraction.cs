namespace Trinica.Entities.Shared;

public record Fraction(string Name)
{
    public static readonly Fraction Homeless = new("Homeless");
    public static readonly Fraction RedHouse = new("Red House");
    public static readonly Fraction OrangeHouse = new("Orange House");
    public static readonly Fraction YellowHouse = new("Yellow House");
    public static readonly Fraction GreenHouse = new("Yellow House");
    public static readonly Fraction BlueHouse = new("Yellow House");
    public static readonly Fraction VioletHouse = new("Violet House");
    public static readonly Fraction PurpleHouse = new("Purple House");
    public static readonly Fraction PinkHouse = new("Pink House");
}
