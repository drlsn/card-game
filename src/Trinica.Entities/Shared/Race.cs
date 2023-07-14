namespace Trinica.Entities.Shared;

public record Race(string Name)
{
    public static readonly Race Magus = new("Magus");
    public static readonly Race Slime = new("Slime");
    public static readonly Race Fairy = new("Fairy");
    public static readonly Race Undead = new("Undead");
    public static readonly Race Shadow = new("Shadow"); // Chowaniec?
    public static readonly Race Construct = new("Construct");
    public static readonly Race Elemental = new("Elemental");
    public static readonly Race Plantoid = new("Plantoid");
    public static readonly Race Dragon = new("Dragon");
    public static readonly Race Angel = new("Angel");
    public static readonly Race Demon = new("Demon");
}
